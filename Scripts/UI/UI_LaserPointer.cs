using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

namespace UI {
    abstract public class UI_LaserPointer : MonoBehaviour {


		public LayerMask UIlayerMask;

        public GameObject laser;
		public GameObject hitPoint;


		private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
		private SteamVR_TrackedObject trackedObj;

        private float _distanceLimit;
		private int sceneIndex; 

		public GameObject HandMenuL;
		public GameObject HandMenuR;

        // Use this for initialization
        void Start()
        {

			trackedObj = GetComponent<SteamVR_TrackedObject> ();
            Initialize();
            
            // register with the LaserlaserInputModule
            if(UI_LaserPointerInputModule.instance == null) {
                new GameObject().AddComponent<UI_LaserPointerInputModule>();
            }
            
			sceneIndex = SceneManager.GetActiveScene ().buildIndex;


            UI_LaserPointerInputModule.instance.AddController(this);
        }

        void OnDestroy()
        {
            if(UI_LaserPointerInputModule.instance != null)
                UI_LaserPointerInputModule.instance.RemoveController(this);
        }

        protected virtual void Initialize() { }
        public virtual void OnEnterControl(GameObject control) { }
        public virtual void OnExitControl(GameObject control) { }
        abstract public bool ButtonDown();
        abstract public bool ButtonUp();

        protected virtual void Update()
        {


            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
			bool bHit = Physics.Raycast(ray, out hitInfo, UIlayerMask);

            float distance = 100.0f;

            if(bHit) {
                distance = hitInfo.distance;
            }

            // ugly, but has to do for now
			if(_distanceLimit > 0.0f) {
                distance = Mathf.Min(distance, _distanceLimit);
                bHit = true;
            }

			Vector2 triggerPosition = controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
			bool triggerTapped = triggerPosition.x > 0.5f;
			if (triggerTapped) { // || HandMenuL.activeSelf || HandMenuR.activeSelf) {
				
				if (this.GetComponent<FixedJoint> ()) {

				} else {
					laser.transform.localScale = new Vector3 (laser.transform.localScale.x, laser.transform.localScale.y,
						distance);				
					laser.transform.localPosition = new Vector3 (0.0f, 0.0f, distance * 0.5f);
					hitPoint.SetActive (true);
					laser.SetActive (true);
				}
			} else if (HandMenuL.activeSelf || HandMenuR.activeSelf) {
				laser.transform.localScale = new Vector3 (laser.transform.localScale.x, laser.transform.localScale.y,
					distance);				
				laser.transform.localPosition = new Vector3 (0.0f, 0.0f, distance * 0.5f);
				hitPoint.SetActive (true);
				laser.SetActive (true);
			} else if (sceneIndex == 0) { // laser always on in load scene
				laser.transform.localScale = new Vector3 (laser.transform.localScale.x, laser.transform.localScale.y,
					distance);				
				laser.transform.localPosition = new Vector3 (0.0f, 0.0f, distance * 0.5f);
				hitPoint.SetActive (true);
				laser.SetActive (true);
			
			}else {
				hitPoint.SetActive (false);
				laser.SetActive (false);
			}
            if(bHit) {
                hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
            }

            // reset the previous distance limit
            _distanceLimit = -1.0f;
        }

        // limits the laser distance for the current frame
        public virtual void LimitLaserDistance(float distance)
        {
            if(distance < 0.0f)
                return;

            if(_distanceLimit < 0.0f)
                _distanceLimit = distance;
            else
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
        }

		 
    }

}