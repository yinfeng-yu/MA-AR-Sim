using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity.CoordinateSystem;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mediapipe.Unity.Tutorial
{
    public class MediaPipeHandTracking : Singleton<MediaPipeHandTracking>
    {
        [SerializeField] private TextAsset _configAsset;
        [SerializeField] private RawImage _screen;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _fps;


        [SerializeField] private int _maxNumHands;

        private CalculatorGraph _graph;
        private ResourceManager _resourceManager;

        private WebCamTexture _webCamTexture;
        private Texture2D _inputTexture;
        private Color32[] _inputPixelData;
        private Texture2D _outputTexture;
        private Color32[] _outputPixelData;

        public Transform leftHandSkeleton;
        public Transform rightHandSkeleton;

        public Transform[] leftHandPoints;
        Vector3[] leftHandPointsTranslations = new Vector3[21];
        public Transform[] leftHandLines;

        public Transform[] rightHandPoints;
        Vector3[] rightHandPointsTranslations = new Vector3[21];
        public Transform[] rightHandLines;

        /// <summary>
        /// The palm length as reference
        /// </summary>
        public float palmLength = 0.1f;

        /// <summary>
        /// Focal length of the camera for hand-tracking.
        /// </summary>
        public float focalLength = 680;

        /// <summary>
        /// The distance from the camera to the hand
        /// </summary>
        public float leftHandDistance;
        public float rightHandDistance;

        /// <summary>
        /// The scale of the tracked palm. Used to calculate the hand distances.
        /// </summary>
        float leftHandScale;
        float rightHandScale;

        /// <summary>
        /// The wrist position
        /// </summary>
        public Vector3 leftHandRootTranslation;
        public Vector3 rightHandRootTranslation;


        public float leftHandDistanceScale = 0.2f;
        public Vector2 leftHandRootScreenPos;

        public float rightHandDistanceScale = 0.2f;
        public Vector2 rightHandRootScreenPos;

        public Vector3 leftPalmNorm;
        public Vector3 leftPalmForward;
        public Vector3 leftPalmRight;

        public Vector3 rightPalmNorm;
        public Vector3 rightPalmForward;
        public Vector3 rightPalmRight;

        public bool flipPalm = false;

        // public HandIK handIK;

        float leftThumbAngleVelocity;
        float leftIndexAngleVelocity;
        float leftMiddleAngleVelocity;
        float leftRingAngleVelocity;
        float leftPinkyAngleVelocity;

        float rightThumbAngleVelocity;
        float rightIndexAngleVelocity;
        float rightMiddleAngleVelocity;
        float rightRingAngleVelocity;
        float rightPinkyAngleVelocity;


        [Header("Thumb")]
        [Range(0, 90f)]
        public float minThumb = 0f;
        [Range(0, 90f)]
        public float maxThumb = 60f;

        [Header("Index")]
        [Range(0, 90f)]
        public float minIndex = 0f;
        [Range(0, 90f)]
        public float maxIndex = 60f;

        [Header("Middle")]
        [Range(0, 90f)]
        public float minMiddle = 0f;
        [Range(0, 90f)]
        public float maxMiddle = 60f;

        [Header("Ring")]
        [Range(0, 90f)]
        public float minRing = 0f;
        [Range(0, 90f)]
        public float maxRing = 60f;

        [Header("Pinky")]
        [Range(0, 90f)]
        public float minPinky = 0f;
        [Range(0, 90f)]
        public float maxPinky = 60f;

        struct MPFinger
        {
            public int root;
            public Finger finger;

            public MPFinger(int root, Finger finger)
            {
                this.root = root;
                this.finger = finger;
            }
        }

        MPFinger[] fingers = new[]
            {
                new MPFinger(1,  new Finger(Handedness.Left, FingerType.Thumb) ),
                new MPFinger(5,  new Finger(Handedness.Left, FingerType.Index) ),
                new MPFinger(9,  new Finger(Handedness.Left, FingerType.Middle)),
                new MPFinger(13, new Finger(Handedness.Left, FingerType.Ring)  ),
                new MPFinger(17, new Finger(Handedness.Left, FingerType.Little)),

                new MPFinger(1,  new Finger(Handedness.Right, FingerType.Thumb) ),
                new MPFinger(5,  new Finger(Handedness.Right, FingerType.Index) ),
                new MPFinger(9,  new Finger(Handedness.Right, FingerType.Middle)),
                new MPFinger(13, new Finger(Handedness.Right, FingerType.Ring)  ),
                new MPFinger(17, new Finger(Handedness.Right, FingerType.Little))
            };


        private IEnumerator Start()
        {
            // leftHandKalmanFilter = new KalmanFilterVector3(Q, R);
            // rightHandKalmanFilter = new KalmanFilterVector3(Q, R);

            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("Web Camera devices are not found");
            }
            var webCamDevice = WebCamTexture.devices[0];
            _webCamTexture = new WebCamTexture(webCamDevice.name, _width, _height, _fps);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.width > 16);

            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);

            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _inputPixelData = new Color32[_width * _height];
            _outputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _outputPixelData = new Color32[_width * _height];

            // _screen.texture = _outputTexture;

            _resourceManager = new StreamingAssetsResourceManager();

            yield return _resourceManager.PrepareAssetAsync("hand_landmark_full.bytes");
            yield return _resourceManager.PrepareAssetAsync("palm_detection_full.bytes");

            var stopwatch = new Stopwatch();

            _graph = new CalculatorGraph(_configAsset.text);
            // var outputVideoStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, "output_video");
            // var multiFaceLandmarksStream = new OutputStream<NormalizedLandmarkListVectorPacket, List<NormalizedLandmarkList>>(_graph, "multi_face_landmarks");

            var handLandmarksStream = new OutputStream<NormalizedLandmarkListVectorPacket, List<NormalizedLandmarkList>>(_graph, "hand_landmarks");
            var handWorldLandmarksStream = new OutputStream<LandmarkListVectorPacket, List<LandmarkList>>(_graph, "hand_world_landmarks");
            var handednessStream = new OutputStream<ClassificationListVectorPacket, List<ClassificationList>>(_graph, "handedness");
            var palmDetectionsStream = new OutputStream<DetectionVectorPacket, List<Detection>>(_graph, "palm_detections");
            var handRectsFromLandmarksStream = new OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>>(_graph, "hand_rects_from_landmarks");
            var handRectsFromPalmDetectionsStream = new OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>>(_graph, "hand_rects_from_palm_detections");

            // outputVideoStream.StartPolling().AssertOk();
            // multiFaceLandmarksStream.StartPolling().AssertOk();

            handLandmarksStream.StartPolling().AssertOk();
            handWorldLandmarksStream.StartPolling().AssertOk();
            handednessStream.StartPolling().AssertOk();
            palmDetectionsStream.StartPolling().AssertOk();
            handRectsFromLandmarksStream.StartPolling().AssertOk();
            handRectsFromPalmDetectionsStream.StartPolling().AssertOk();

            // var imageSource = ImageSourceProvider.ImageSource;
            // yield return imageSource.Play();
            // 
            // if (!imageSource.isPrepared)
            // {
            //     Logger.LogError("Failed to start ImageSource, exiting...");
            //     yield break;
            // }

            var sidePacket = new SidePacket();

            sidePacket.Emplace("num_hands", new IntPacket(_maxNumHands));
            Logger.LogInfo($"Max Num Hands = {_maxNumHands}");

            _graph.StartRun(sidePacket).AssertOk();

            stopwatch.Start();

            var screenRect = _screen.GetComponent<RectTransform>().rect;

            while (true)
            {
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_inputPixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
                _graph.AddPacketToInputStream("input_video", new ImageFramePacket(imageFrame, new Timestamp(currentTimestamp))).AssertOk();

                yield return new WaitForEndOfFrame();

                if (handednessStream.TryGetNext(out var handednesses))
                {
                    if (handednesses.Count > 0)
                    {
                        string h = "";
                        for (int j = 0; j < handednesses.Count; j++)
                        {
                            string handedness = handednesses[j].Classification.ToString().Contains("Left") ? "Right" : "Left";
                            h += handedness;
                            h += " ";
                        }
                        // string h = handednesses.Count == 1 ? handednesses[0].ToString() : handednesses[0].ToString() + ", " + handednesses[1].ToString();
                        // Debug.Log($"handedness count = {handednesses.Count}, handedness: {h}");
                        // Debug.Log($"handedness: {h}");
                    }
                }

                // TrackSingleHand(ref handLandmarksStream, "Left");
                // TrackSingleHand(ref handLandmarksStream, "Right");

                if (handLandmarksStream.TryGetNext(out var handLandmarks))
                {
                    // Debug.Log("detected");
                    if (handLandmarks != null && handLandmarks.Count > 0)
                    {
                        bool leftHandDetected = false;
                        bool rightHandDetected = false;
                        for (int j = 0; j < handLandmarks.Count; j++)
                        {
                            var landmarks = handLandmarks[j];
                            // For the webcam (the camera resides in the same side of the hands), the handedness is inverted.
                            string handedness = handednesses[j].Classification.ToString().Contains("Left") ? "Right" : "Left";
                            // Debug.Log($"handedness: {handedness}");

                            if (handedness == "Left" && !leftHandDetected)
                            {
                                // leftHandDetected = true;
                                leftHandRootScreenPos = screenRect.GetPoint(landmarks.Landmark[0]);
                                leftHandRootTranslation = screenRect.GetPoint(landmarks.Landmark[0]) / 100;
                                for (int i = 0; i < landmarks.Landmark.Count; i++)
                                {
                                    var landmark = landmarks.Landmark[i];
                                    // Debug.Log($"Unity Local Coordinates: {screenRect.GetPoint(landmark)}, Image Coordinates: {landmark}");
                                    // points[i].transform.localPosition = screenRect.GetPoint(landmark) / 100;
                                    leftHandPointsTranslations[i] = (screenRect.GetPoint(landmark) / 100) - leftHandRootTranslation;
                                }

                                // Now the hand landmarks are updated.
                                leftHandScale = UpdateHandOrientation("Left", leftHandPointsTranslations);

                                for (int i = 0; i < 21; i++)
                                {
                                    leftHandPointsTranslations[i] *= leftHandScale;

                                    leftHandPoints[i].transform.localPosition = leftHandPointsTranslations[i];
                                }

                                foreach (var finger in fingers)
                                {
                                    float angle;
                                    if (finger.finger.handedness == Handedness.Left)
                                    {
                                        // angle = CalculateFingerAngles(finger, leftHandPointsTranslations, leftPalmForward, leftPalmRight)[1];
                                        // SetFingerPercentage(finger, angle, "Left");
                                        FingerController.Instance.UpdateFingerAngles(finger.finger, CalculateFingerAngles(finger, leftHandPointsTranslations, leftPalmForward, leftPalmRight));
                                    }
                                }

                                leftHandRootTranslation *= leftHandScale;
                                leftHandDistance = focalLength * leftHandScale * leftHandDistanceScale;
                                UpdateLines(leftHandLines, leftHandPointsTranslations);

                                break;
                            }

                            else if (handedness == "Right" && !rightHandDetected)
                            {
                                // rightHandDetected = true;
                                rightHandRootScreenPos = screenRect.GetPoint(landmarks.Landmark[0]);
                                rightHandRootTranslation = screenRect.GetPoint(landmarks.Landmark[0]) / 100;
                                // Debug.Log($"screenPos: {rightHandRootScreenPos}, root translation: {rightHandRootTranslation}");
                                for (int i = 0; i < landmarks.Landmark.Count; i++)
                                {
                                    var landmark = landmarks.Landmark[i];
                                    rightHandPointsTranslations[i] = (screenRect.GetPoint(landmark) / 100) - rightHandRootTranslation;
                                }

                                // Now the hand landmarks are updated.
                                rightHandScale = UpdateHandOrientation("Right", rightHandPointsTranslations);

                                for (int i = 0; i < 21; i++)
                                {
                                    rightHandPointsTranslations[i] *= rightHandScale;

                                    rightHandPoints[i].transform.localPosition = rightHandPointsTranslations[i];
                                }

                                foreach (var finger in fingers)
                                {
                                    float angle;
                                    if (finger.finger.handedness == Handedness.Right)
                                    {
                                        // angle = CalculateFingerAngles(finger, rightHandPointsTranslations, rightPalmForward, rightPalmRight)[1];
                                        // SetFingerPercentage(finger, angle, "Right");
                                    }
                                }

                                rightHandRootTranslation *= rightHandScale;
                                rightHandDistance = focalLength * rightHandScale * rightHandDistanceScale;
                                UpdateLines(rightHandLines, rightHandPointsTranslations);

                                break;
                            }
                        }
                    }
                }


            }
        }
        

        float UpdateHandOrientation(string handedness, Vector3[] handPointsTranslations)
        {
            var trackedPalmLength = Vector3.Distance(handPointsTranslations[5], handPointsTranslations[0]);
            float handScale = palmLength / trackedPalmLength;

            Vector3 palmVec1 = handPointsTranslations[5] - handPointsTranslations[0];
            Vector3 palmVec2 = handPointsTranslations[17] - handPointsTranslations[0];

            if (handedness == "Left")
            {
                leftPalmNorm = (Vector3.Cross(palmVec1, palmVec2) * (flipPalm ? -1 : 1)).normalized;
                leftPalmForward = (palmVec1 + palmVec2).normalized;
                leftPalmRight = -Vector3.Cross(leftPalmForward, leftPalmNorm);
            }
            else
            {
                rightPalmNorm = (Vector3.Cross(palmVec1, palmVec2) * (flipPalm ? -1 : 1)).normalized;
                rightPalmForward = (palmVec1 + palmVec2).normalized;
                rightPalmRight = -Vector3.Cross(rightPalmForward, rightPalmNorm);
            }

            return handScale;
        }

        float[] CalculateFingerAngles(MPFinger finger, Vector3[] handPointsTranslations, Vector3 palmForward, Vector3 palmRight)
        {
            if (finger.finger.fingerType == FingerType.Thumb)
            {
                Vector3 vec1 = handPointsTranslations[2] - handPointsTranslations[0];
                Vector3 vec2 = handPointsTranslations[5] - handPointsTranslations[0];

                palmForward = (vec1 + vec2).normalized;
                Vector3 thumbNorm = (Vector3.Cross(vec1, vec2) * (flipPalm ? -1 : 1)).normalized;
                palmRight = -Vector3.Cross(palmForward, thumbNorm);
            }

            var root = finger.root;
            Vector3 finger1 = Vector3.ProjectOnPlane((handPointsTranslations[root + 1] - handPointsTranslations[root]), palmRight);
            Vector3 finger2 = Vector3.ProjectOnPlane((handPointsTranslations[root + 2] - handPointsTranslations[root + 1]), palmRight);
            Vector3 finger3 = Vector3.ProjectOnPlane((handPointsTranslations[root + 3] - handPointsTranslations[root + 2]), palmRight);
            float angle1 = - Vector3.SignedAngle(finger1, palmForward, palmRight);
            float angle2 = - Vector3.SignedAngle(finger2, finger1, palmRight);
            float angle3 = - Vector3.SignedAngle(finger3, finger2, palmRight);
            // Debug.Log($"angle1: {angle1}, angle2: {angle2}, angle3: {angle3}");

            FingerController.Instance.UpdateFingerAngles(finger.finger, new[] { angle1, angle2, angle3 });
            return new[] { angle1, angle2, angle3 };
        }

        

        void UpdateLines(Transform[] handLines, Vector3[] handPointsTranslations)
        {
            for (int i = 0; i < 21; i++)
            {
                if (handPointsTranslations[i][0] == float.NaN) return;

                if (i == 4 || i == 16)
                {
                    float length = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[0]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[0];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);
                }

                else if (i == 8 || i == 12)
                {
                    float length = Vector3.Distance(handPointsTranslations[i - 3], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i - 3]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[i - 3];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);

                }

                else if (i == 20)
                {
                    float length = Vector3.Distance(handPointsTranslations[13], handPointsTranslations[17]);
                    Vector3 forward = (handPointsTranslations[17] - handPointsTranslations[13]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[13];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);

                }

                else
                {
                    float length = Vector3.Distance(handPointsTranslations[i], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[i];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);

                }
            }
        }

        private void OnDestroy()
        {
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
            }

            if (_graph != null)
            {
                try
                {
                    _graph.CloseInputStream("input_video").AssertOk();
                    _graph.WaitUntilDone().AssertOk();
                }
                finally
                {

                    _graph.Dispose();
                }
            }
        }

        // private SidePacket BuildSidePacket(ImageSource imageSource)
        // {
        //     var sidePacket = new SidePacket();
        // 
        //     SetImageTransformationOptions(sidePacket, imageSource, true);
        //     // sidePacket.Emplace("model_complexity", new IntPacket((int)modelComplexity));
        //     sidePacket.Emplace("num_hands", new IntPacket(_maxNumHands));
        // 
        //     // Logger.LogInfo(TAG, $"Model Complexity = {modelComplexity}");
        //     Logger.LogInfo($"Max Num Hands = {_maxNumHands}");
        // 
        //     return sidePacket;
        // }

        protected void SetImageTransformationOptions(SidePacket sidePacket, ImageSource imageSource, bool expectedToBeMirrored = false)
        {
            // NOTE: The origin is left-bottom corner in Unity, and right-top corner in MediaPipe.
            RotationAngle rotation = imageSource.rotation.Reverse();
            var inputRotation = rotation;
            var isInverted = CoordinateSystem.ImageCoordinate.IsInverted(rotation);
            var shouldBeMirrored = imageSource.isHorizontallyFlipped ^ expectedToBeMirrored;
            var inputHorizontallyFlipped = isInverted ^ shouldBeMirrored;
            var inputVerticallyFlipped = !isInverted;

            if ((inputHorizontallyFlipped && inputVerticallyFlipped) || rotation == RotationAngle.Rotation180)
            {
                inputRotation = inputRotation.Add(RotationAngle.Rotation180);
                inputHorizontallyFlipped = !inputHorizontallyFlipped;
                inputVerticallyFlipped = !inputVerticallyFlipped;
            }

            Logger.LogDebug($"input_rotation = {inputRotation}, input_horizontally_flipped = {inputHorizontallyFlipped}, input_vertically_flipped = {inputVerticallyFlipped}");

            sidePacket.Emplace("input_rotation", new IntPacket((int)inputRotation));
            sidePacket.Emplace("input_horizontally_flipped", new BoolPacket(inputHorizontallyFlipped));
            sidePacket.Emplace("input_vertically_flipped", new BoolPacket(inputVerticallyFlipped));
        }
    }
}