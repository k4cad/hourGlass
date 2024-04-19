using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class baseScript : MonoBehaviour {

	private TextMeshPro batchCountTMP;

	private static int maxRadials = 9;
	private static Vector2[] radialPnts = new Vector2[maxRadials];
	private static int radials = 9;

	private static int layerCount = 50;
	private static int batchCount = layerCount * (1 +radials);
	private static int maxBatch = 100;
	private int currentBatch;
	private GameObject gosg;
	private GameObject[] duplicates = new GameObject[batchCount * maxBatch];
	private bool[] grainDropped = new bool[batchCount * maxBatch];

	private Vector3[] neckPoint = new Vector3[batchCount * maxBatch];
	private string[] neckPointName = new string[batchCount * maxBatch];
	private Vector3[] basePoint = new Vector3[batchCount * maxBatch];
	private string[] basePointName = new string[batchCount * maxBatch];



	private static MeshRenderer fillPlugMR; 
	private static CapsuleCollider fillPlugCC; 

	private static MeshRenderer middlePlugMR; 
	private static MeshCollider middlePlugMC; 
	private static MeshCollider bottomHalfHour3MC; 

    private static Transform middlePlugTR; 
	private static Transform counterBottomTR; 
	private static Transform counterTopTR;

	private static Transform hourGlassTR; 
	private static Transform testSphereTR; 
	private static Transform testSphere2TR; 

	private bool  rotatingHourGlass = false;
	private float rotated;
	private float rotateSpeed;
	private bool  rotateAccelerating, rotateDecelerating;
	private float rotateSlowDown;

	private bool  liftingHourGlass = false;
	private float lifted, liftHeight;
	private float liftSpeed, liftSpeedMax, liftSpeedIncrement;
	private bool  liftAccelerating, liftDecelerating;
	private bool  liftWaiting = false;


	private bool  loweringHourGlass = false;
	private float lowered, lowerHeight;
	private float lowerSpeed, lowerSpeedMax, lowerSpeedIncrement;
	private bool  lowerAccelerating, lowerDecelerating;
	private bool  lowerWaiting = false;

	private bool  loading = false;
	private bool  loadWaiting;
	private float loadWaitTime = 0;
	private int   loads;
	private int   targetLoads;



	private bool waiting = false, waiting2 = false, waiting3 = false, waiting4 = false, waiting5 = false, waiting6 = false, waiting7 = false, waiting8 = false;
	private float waitTime = 0, waitTime2 = 0, waitTime3 = 0, waitTime4 = 0, waitTime5 = 0, waitTime6 = 0, waitTime7 = 0, waitTime8 = 0;
	private float liftWait = 0;
	private float rattleTime = 0;
	private bool rattling = false;


	private bool holePlugged = false;
	private bool dropping = false;
	private bool dropping2 = false;
	private bool allDropped = false;
	private bool middleOpen;

	private int stuck;
	private int fallen;

	private float dropStart = 27;


	private int PHASE = 3;

	private int batches = 4;
	private int origBatches;

	private int moveBatch;
	private int targetBatch;
	private bool processing = false;

	private float middleY;


	private static int bands = 200;
	private Vector2[]  bandRange = new Vector2[bands];
	private int[]      bandItems = new int[bands];
	private float[]    bandRadius = new float[bands];
	private float      maxBulge = 0.05f;  // 0.1f  0.2f
	private float      baseBandReduction = 0.001f; // so dont get stuck on bottom ???


	private bool jumping = false;
	private float jumpSpeed, jumpHeight, jumped;

	private bool      calibrating = false;
	private bool      calibrateReadPoints = false;
	private bool      calibrateWaiting = false, calibrateWaiting2 = false, calibrateWaiting3 = false;
	private bool      calibrateRotating = false, calibrateRotating2 = false, calibrateRotating2Finished = false;
	private float     calibrateWaitTime = 0, calibrateWaitTime2 = 0, calibrateWaitTime3 = 0;
	private bool      calibrateMeasureBase = false, calibrateMeasureNeck = false;
	private float     calibrateBaseDist = 0, calibrateNeckDist = 0, calibrateRotated = 0, calibrateRotated2 = 0;
	private float     calibrateRotateSpeed;
	private Vector2[] calibrateRange = new Vector2[bands];
	private float[]   calibrateRadius = new float[bands];
	private int       calibrateTurns, calibrateTargetTurns;


	private static int PROFILE_A = 1;
	private static int PROFILE_B = 2;
	private static int PROFILE_C = 3;
	private int profile;







    // Start is called before the first frame update
    void Start()
    {
		
		string objectName = "sandGrain_0";
		gosg = GameObject.Find (objectName);

		if (PHASE == 3) {
			//readBandData ("bandData2.txt", -0.005f, 169, 6);
			//writeBandData ("bandDataNew.txt", bandRange, bandRadius);
			//writeBandScript ("bandLineNew.scr", bandRange, bandRadius);
			//readBandData ("calibratedBandData3.txt", 0.0f, -1, -1);





			profile = PROFILE_A;

            if (profile == PROFILE_A)
			    readBandData ("profile_A.txt", 0.0f, -1, -1);
            else if (profile == PROFILE_B)
			    readBandData ("profile_B.txt", 0.0f, -1, -1);
            else if (profile == PROFILE_C)
			    readBandData ("profile_C.txt", 0.0f, -1, -1);

			GameObject GO = GameObject.Find ("middlePlug");
			if (GO == null)
				Debug.Log ("NOT GOT middlePlug");
			else { 
				middlePlugMR = GO.GetComponent<MeshRenderer> ();
				middlePlugMC = GO.GetComponent<MeshCollider> ();
				middlePlugTR = GO.GetComponent<Transform> ();
			}
			middleY = middlePlugTR.transform.position.y;
			Debug.Log ("NECK middleY: " + middleY);

			GO = GameObject.Find ("counterBottom");
			if (GO == null)
				Debug.Log ("NOT GOT counterBottom");
			else {
				counterBottomTR = GO.GetComponent<Transform> ();
				GO.GetComponent<MeshRenderer> ().enabled = false;
				Debug.Log ("GOT counterBottomTR");
			}

			GO = GameObject.Find ("counterTop");
			if (GO == null)
				Debug.Log ("NOT GOT counterTop");
			else {
				counterTopTR = GO.GetComponent<Transform> ();
				GO.GetComponent<MeshRenderer> ().enabled = false;
				Debug.Log ("GOT counterTopTR");
			}

			GO = GameObject.Find ("hourGlass");
			if (GO == null)
				Debug.Log ("NOT GOT hourGlass");
			else {
				hourGlassTR = GO.GetComponent<Transform> ();
				Debug.Log ("GOT hourGlassTR");
			}

			GO = GameObject.Find ("batchCountTMP");
			if (GO == null)
				Debug.Log ("NOT GOT batchCountTMP");
			else {
				batchCountTMP = GO.GetComponent<TextMeshPro> ();
				batchCountTMP.enabled = false;
			}

			setupProfile ();





			//calibrating = true;
			//calibrateReadPoints = true;



			// to get 20 batches in top
			//loading = true;
			//loads = 0;
			//targetLoads = 20;


			// to get 20 batches in bottom
			//readStartPoints ("profileB_t20.txt", 0, 5, 0.0f);
			//batches = 5;
			//waiting7 = true;

			//PHASE = 0;
			//Debug.Log ("SET PHASE = 0");




			//Time.timeScale = 60;


			// read 20 batches then turn 180
			readStartPoints ("profileA_b20.txt", 0, 20, 0.0f);
			batches = 20;
			waiting5 = true;
            
            
            
			//Time.timeScale = 50;

			// read 20 turned batches then shake
			//readStartPoints ("profileA_turned20_2.txt", 0, 20, 0.0f);
			//batches = 20;
			//waiting5 = true;
			//Time.timeScale = 50;

		}


		if (PHASE == 1) {
			float xPos = 0f;
			float yPos = dropStart;
			float zPos = 0f;

			//radials = 9;
			float rad = 0.5f;
			float angInc = 360.0f / radials;
			float angle = 0.0f;
			for (int r = 0; r < radials; r++) {
				Vector2 cartCoords = polarToCartesian (rad, angle * Mathf.Deg2Rad);
				//Debug.Log ("cart XY : " + cartCoords.x + ", " + cartCoords.y);
				radialPnts [r] = cartCoords;
				angle = angle + angInc;
			}

			currentBatch = 1;
			GameObject GO = GameObject.Find ("batchCountTMP");
			if (GO == null)
				Debug.Log ("NOT GOT batchCountTMP");
			else {
				batchCountTMP = GO.GetComponent<TextMeshPro> ();
				batchCountTMP.SetText ("batchCount: " + currentBatch);
			}

			GO = GameObject.Find ("fillPlug");
			if (GO == null)
				Debug.Log ("NOT GOT fillPlug");
			else {
				fillPlugMR = GO.GetComponent<MeshRenderer> ();
				fillPlugCC = GO.GetComponent<CapsuleCollider> ();
				Debug.Log ("GOT fillPlugMR and fillPlugCC");
			}

			GO = GameObject.Find ("middlePlug");
			if (GO == null)
				Debug.Log ("NOT GOT middlePlug");
			else {
				middlePlugMR = GO.GetComponent<MeshRenderer> ();
				middlePlugMC = GO.GetComponent<MeshCollider> ();
				middlePlugTR = GO.GetComponent<Transform> ();
				Debug.Log ("GOT middlePlugMR and middlePlugMC and middlePlugTR");
			}

			int idx = 0;
			for (int g = 0; g < layerCount; g++) {
				duplicates [(currentBatch - 1) * batchCount + idx] = Instantiate (gosg);
				string name = "sandGrain_B" + currentBatch + "_" + (idx + 1);
				duplicates [(currentBatch - 1) * batchCount + idx].name = name;
				GameObject dup = GameObject.Find (name);
				dup.transform.position = new Vector3 (xPos, yPos, zPos);
				idx = idx + 1;

				for (int r = 0; r < radials; r++) {
					duplicates [(currentBatch - 1) * batchCount + idx] = Instantiate (gosg);
					string radName = "sandGrain_B" + currentBatch + "_" + (idx + 1);
					duplicates [(currentBatch - 1) * batchCount + idx].name = radName;
					GameObject radDup = GameObject.Find (radName);
					radDup.transform.position = new Vector3 (radialPnts [r].x, yPos, radialPnts [r].y);
					idx = idx + 1;
				}

				yPos = yPos + 0.5f;
			}
		}

		if (PHASE == 2) {

			GameObject GO = GameObject.Find ("middlePlug");
			if (GO == null)
				Debug.Log ("NOT GOT middlePlug");
			else 
				middlePlugTR = GO.GetComponent<Transform> ();
			middleY = middlePlugTR.transform.position.y;
			Debug.Log ("NECK middleY: " + middleY);


			readStartPoints ("startPointsb40.txt", 0, 40, 0.0f);
			batches = 40;
			getNeckPoints ();


			//getNeckBands(middleY);

			origBatches = batches;


			moveBatch = batches;
			waiting2 = true;
			Time.timeScale = 4.0f;
		}


        
    }

    // Update is called once per frame
	void Update() {



        // JUMP============================================
        // JUMP============================================
        // JUMP============================================
        // JUMP============================================


		/*
		if (Input.GetKey (KeyCode.J) && !jumping) { // JUMP
            jumpSpeed = 4.0f; // units/sec
            jumpHeight = 0.3f;
            jumped = 0.0f;
            jumping = true;
			Debug.Log ("set Jumping... ");

            
		}

        if (jumping) {
			Vector3 hourGlassPoint = hourGlassTR.transform.position;
            float dy = jumpSpeed * Time.deltaTime;
			if (jumped + dy >= jumpHeight) {
				dy = jumpHeight - jumped;
				jumping = false;
			}
			jumped = jumped + dy;
			Debug.Log ("Jumped: " + jumped);
			doPushBack ("Jump", true, false, false);
			//newPoint = new Vector3 (hourGlassPoint.x, hourGlassPoint.y + dy, hourGlassPoint.z);
			hourGlassTR.transform.position = new Vector3 (hourGlassPoint.x, hourGlassPoint.y + dy, hourGlassPoint.z);
        }
        */

        // JUMP============================================
        // JUMP============================================
        // JUMP============================================
        // JUMP============================================








		if (PHASE == 3) {

			if (calibrating) {
				if (profile == PROFILE_A)
					calibrateA ("startPointsb1.txt", 1, 18.0f, 2, 2.0f, "profile_A.txt", "profile_A.scr");
				else if (profile == PROFILE_B)
					calibrateA ("startPointsb1.txt", 1, 9.0f, 2, 2.0f, "profile_B.txt", "profile_B.scr");
				else if (profile == PROFILE_C)
					calibrateA ("startPointsb1.txt", 1, 18.0f, 2, 2.0f, "profile_C.txt", "profile_C.scr");
			
			} else {


				// new METHOD use
				  
				// SEQUENCER








				// ========================  WAIT BEFORE TURN  ========================
				if (waiting5) {
					waitTime5 = waitTime5 + Time.deltaTime;
					if (waitTime5 > 1.0f) {
						waiting5 = false;

						rotatingHourGlass = true;
						rotated = 0.0f; 
						rotateSpeed = 0.0f; // deg / sec
						rotateAccelerating = true;
						rotateSlowDown = 1000;  // angle at which to slow down 
						rotateDecelerating = false;
						Debug.Log ("done Waiting5..");
					}
				}

				// ========================  TURN  ========================
				if (rotatingHourGlass) {
					float rotateSpeedIncrement = 25.0f; // deg / sec

					if (rotateAccelerating) { 
						rotateSpeed = rotateSpeed + rotateSpeedIncrement * Time.deltaTime; // deg / sec
						if (rotateSpeed > 45) {
							rotateSpeed = 45.0f;
							rotateAccelerating = false;
							Debug.Log ("MAX SPEED at rotated: " + rotated);
							rotateSlowDown = 180.0f - rotated * 0.9f; 
							// angle at which to start slow down (0.8 means wont slow down completely so will be small jerk) - last speed = 20.4
							// angle at which to start slow down (0.9 means wont slow down completely so will be small jerk) - last speed = 15.0
						}
					}
					if (rotated >= rotateSlowDown) {
						rotateSlowDown = 1000;
						rotateDecelerating = true;   
					}
					if (rotateDecelerating) { 
						rotateSpeed = rotateSpeed - rotateSpeedIncrement * Time.deltaTime; // deg / sec
						if (rotateSpeed < 1.0f) {
							rotateSpeed = 1.0f;
							rotateDecelerating = false;
						}
					}

					float rot = rotateSpeed * Time.deltaTime;
					rotated = rotated + rot; 
					if (rotated >= 180f) {
						rotatingHourGlass = false;
						Debug.Log ("done rotating..");
						rot = rot - (rotated - 180);
						waiting6 = true;
					}
					Debug.Log ("rotated: " + rotated + "  rotateSpeed: " + rotateSpeed);
					hourGlassTR.Rotate (rot, 0, 0);
					doPushBack ("Rotating", true, false, false);
				}

				// ========================  WAIT BEFORE RATTLE  ========================
				if (waiting6) {
					// better if know when can stop waiting
					// grains stopped falling when maxSpeed < 4.0 
					float maxSpeed = doPushBack ("DoneRot", false, false, true);
					Debug.Log ("WT6: " + waitTime6 + "  MS: " + maxSpeed);
					waitTime6 = waitTime6 + Time.deltaTime;
					if (maxSpeed < 4.0f) {
						waiting6 = false;
						rattling = true;
						Debug.Log ("done Waiting6..");
						//writeStartPoints ("profileA_turned20_2.txt");
					}
				}

				// ========================  RATTLE  ========================
				if (rattling) {
					rattleTime = rattleTime + Time.deltaTime;
					shakeAllGrains (0.08f);
					Debug.Log ("RT: " + rattleTime);
					doPushBack ("Rattle", true, false, false);
					if (rattleTime >= 0.2f) {
						rattling = false;
						waiting7 = true;
					}
				}

				// ========================  WAIT AFTER RATTLE  ========================
				if (waiting7) {
					waitTime7 = waitTime7 + Time.deltaTime;
					float maxSpeed = doPushBack ("postRattle", true, false, true);
					Debug.Log ("WT7: " + waitTime7 + "  MS: " + maxSpeed);
					if (maxSpeed < 4.0f) {
						waiting7 = false;
						dropping = true;
						Debug.Log ("done Waiting7..");
					}
				}

				// ========================  DROP  ========================
				if (dropping) {
					if (!middleOpen) {
						allDropped = false;
						middlePlugMR.enabled = false;
						middlePlugMC.enabled = false;
						middleOpen = true;
					}

					int dropped = 0;
					for (int b = 1; b <= batches; b++) {
						for (int d = 0; d < batchCount; d++) {
							int idx = (b - 1) * batchCount + d;
							if (grainDropped [idx])
								dropped = dropped + 1;
							else if (duplicates [idx].transform.position.y < 13.0f) {
								grainDropped [idx] = true;
								dropped = dropped + 1;
							}
						}
					}
					Debug.Log ("dropped = " + dropped);
					if (dropped == batches * batchCount) {
						dropping = false;
						waiting = true;
						allDropped = true;
						Debug.Log ("set ALLDROPPED");
					}
				}

				// ========================  WAIT AFTER ALL DROPPED  ========================
				if (waiting) {
					waitTime = waitTime + Time.deltaTime;
					float maxSpeed = doPushBack ("postDrop", true, false, true);
					Debug.Log ("WT: " + waitTime + "  MS: " + maxSpeed);
					if (maxSpeed < 4.0f) {
						waiting = false;
						Debug.Log ("done Waiting..");
						writeStartPoints ("profileB_b20.txt");
					}
				}


				/////////// ================ UNUSED FUNCTIONS ===================== ///////////
				/////////// ================ UNUSED FUNCTIONS ===================== ///////////
				/////////// ================ UNUSED FUNCTIONS ===================== ///////////

				if (loweringHourGlass) {
					Vector3 hourGlassPoint = hourGlassTR.transform.position;
					if (lowerAccelerating) {
						lowerSpeed = lowerSpeed + lowerSpeedIncrement * Time.deltaTime; // deg / sec
						if (lowerSpeed > lowerSpeedMax) {
							lowerSpeed = lowerSpeedMax;
							lowerAccelerating = false;
						}
					}
					if (lowerDecelerating) { 
						lowerSpeed = lowerSpeed - lowerSpeedIncrement * Time.deltaTime; // deg / sec
						if (lowerSpeed < 0.5f)  {
							lowerSpeed = 0.5f;
							lowerDecelerating = false;
						}
					}
					float dy = lowerSpeed * Time.deltaTime;
					if (lowered + dy >= lowerHeight) {
						dy = lowerHeight - lowered;
						loweringHourGlass = false;
						//lowerWaiting = true;
					}
					lowered = lowered + dy;
					if (lowered > lowerHeight / 2) {
						if (lowerAccelerating)
							Debug.Log ("===================== NOT EXPECTED ------ 2");
						lowerAccelerating = false;
						lowerDecelerating = true;
					}
					Debug.Log ("Lowered: " + lowered + "  speed: " + lowerSpeed);
					doPushBack ("Lower", true, false, false);
					hourGlassTR.transform.position = new Vector3 (hourGlassPoint.x, hourGlassPoint.y - dy, hourGlassPoint.z);
				}

				if (liftingHourGlass) {
					Vector3 hourGlassPoint = hourGlassTR.transform.position;
					if (liftAccelerating) {
						liftSpeed = liftSpeed + liftSpeedIncrement * Time.deltaTime; // deg / sec
						if (liftSpeed > liftSpeedMax) {
							liftSpeed = liftSpeedMax;
							liftAccelerating = false;
						}
					}
					if (liftDecelerating) { 
						liftSpeed = liftSpeed - liftSpeedIncrement * Time.deltaTime; // deg / sec
						if (liftSpeed < 0.5f)  {
							liftSpeed = 0.5f;
							liftDecelerating = false;
						}
					}
					float dy = liftSpeed * Time.deltaTime;
					if (lifted + dy >= liftHeight) {
						dy = liftHeight - lifted;
						liftingHourGlass = false;
						liftWaiting = true;
					}
					lifted = lifted + dy;
					if (lifted > liftHeight / 2) {
						if (liftAccelerating)
							Debug.Log ("===================== NOT EXPECTED");
						liftAccelerating = false;
						liftDecelerating = true;
					}
					Debug.Log ("Lifted: " + lifted + "  speed: " + liftSpeed);
					doPushBack ("Lift", true, false, false);
					hourGlassTR.transform.position = new Vector3 (hourGlassPoint.x, hourGlassPoint.y + dy, hourGlassPoint.z);
				}

				if (liftWaiting) {
					liftWait = liftWait + Time.deltaTime;
					if (liftWait > 0.5f) {
						liftWaiting = false;
						loweringHourGlass = true;
						lowerSpeedIncrement = 6.0f; // unit / sec
						lowerHeight = 2.0f;
						lowered = 0.0f;
						lowerSpeed = 0;
						lowerSpeedMax = 4;
						lowerAccelerating = true;
						lowerDecelerating = false;
					}
					doPushBack ("LiftWait", true, false, false);
				}

				// to get to stage of 40 batches dropped - natural pile at bottom
				// start with endPointsb4 (4 batches at bottom) and start with startPointsb4 at top - let fall - then save as endPointsb8 (8 batches)
				// then start with endPointsb8 (4 batches at top) then drop another 4 batches in top (middle closed) let fall (timescale 50) - then save as endPointsb8 (8 batches)
				if (loading) {
					readStartPoints ("startPointsb1.txt", loads, 1, 4.0f);
					loading = false;
					loadWaiting = true;
					loadWaitTime = 0.0f;
					loads = loads + 1;
					batches = loads;
					Debug.Log ("loads: " + loads);
				}
				if (loadWaiting) {
					loadWaitTime = loadWaitTime + Time.deltaTime;
					if (loadWaitTime > 5f) {
						loadWaiting = false;
						if (loads == targetLoads) {
							//waiting7 = true;
							//dropping = false;
							writeStartPoints ("profileB_t20.txt");
							Debug.Log ("Done Loading");
						} else
							loading = true;
					}
				}

				if (jumping) {
					Vector3 hourGlassPoint = hourGlassTR.transform.position;
					float dy = jumpSpeed * Time.deltaTime;
					if (jumped + dy >= jumpHeight) {
						dy = jumpHeight - jumped;
						jumping = false;
					}
					jumped = jumped + dy;
					Debug.Log ("Jumped: " + jumped);
					doPushBack ("Jump", true, false, false);
					//newPoint = new Vector3 (hourGlassPoint.x, hourGlassPoint.y + dy, hourGlassPoint.z);
					hourGlassTR.transform.position = new Vector3 (hourGlassPoint.x, hourGlassPoint.y + dy, hourGlassPoint.z);
				}

				/////////// ================ UNUSED FUNCTIONS ===================== ///////////
				/////////// ================ UNUSED FUNCTIONS ===================== ///////////
				/////////// ================ UNUSED FUNCTIONS ===================== ///////////


			} /// end else (not calibrating)


		}  // PHASE 3




		if (PHASE == 1) {
			bool doBatch = false;

			if (currentBatch < batches) {
				string name = "sandGrain_B" + currentBatch + "_" + batchCount;
				GameObject dup = GameObject.Find (name);
				if (dup.transform.position.y < dropStart - 0.5f)
					doBatch = true;
			} else
				doBatch = false;
			if (holePlugged)
				doBatch = false;

			if (doBatch /* && Input.GetKey (KeyCode.Space) */) {
				currentBatch = currentBatch + 1;				

				batchCountTMP.SetText("batchCount: " + currentBatch);

				int idx = 0;
				float xPos = 0f;
				float yPos = dropStart;
				float zPos = 0f;

				for (int g = 0; g < layerCount; g++) {
					duplicates [(currentBatch - 1) * batchCount + idx] = Instantiate (gosg);
					string name = "sandGrain_B" + currentBatch + "_" + (idx + 1);
					duplicates [(currentBatch - 1) * batchCount + idx].name = name;
					GameObject dup = GameObject.Find (name);
					dup.transform.position = new Vector3 (xPos, yPos, zPos);
					idx = idx + 1;

					for (int r = 0; r < radials; r++) {
						duplicates [(currentBatch - 1) * batchCount + idx] = Instantiate (gosg);
						string radName = "sandGrain_B" + currentBatch + "_" + (idx + 1);
						duplicates [(currentBatch - 1) * batchCount + idx].name = radName;
						GameObject radDup = GameObject.Find (radName);
						radDup.transform.position = new Vector3 (radialPnts [r].x, yPos, radialPnts [r].y);
						idx = idx + 1;
					}

					yPos = yPos + 0.5f;

				}
			}

			if (currentBatch == batches && !waiting && !rotatingHourGlass && !allDropped) {
				string name = "sandGrain_B" + currentBatch + "_" + batchCount;
				GameObject dup = GameObject.Find (name);
				if (dup.transform.position.y < 24.5f) {
					waiting = true;
					waitTime = 0.0f;
					fillPlugMR.enabled = true;
					fillPlugCC.enabled = true;
					holePlugged = true;
					Debug.Log ("set Waiting..");
				}
			}

			if (waiting) {
				waitTime = waitTime + Time.deltaTime;
				if (waitTime > 4.0f) {
					waiting = false;
					Debug.Log ("done Waiting..");

					// get neck Points
					//float middleZ = middlePlugTR.transform.position.z;
					//Debug.Log ("NECK middleZ: " + middleZ);

					/*
				int neckIdx = 0;
				Vector3[] neckPoint = new Vector3[batchCount * maxBatch];
				Vector3 tempNeckPoint;
				for (int b = 1; b <= batches; b++) {
					for (int d = 1; d <= batchCount; d++) {
						string name = "sandGrain_B" + b + "_" + d;
						GameObject dup = GameObject.Find (name);
						if (dup.transform.position.y > middleZ) {
							Debug.Log ("NECK: " + name);
							neckPoint [neckIdx] = dup.transform.position;
							neckIdx = neckIdx + 1;
						}
					}
				}

				// sort neck Points
				bool swapped;
				do {
					swapped = false;
					for (int s = 0; s < neckIdx - 1; s++) {
						if (neckPoint [s].y > neckPoint [s + 1].y) { // swap 
							tempNeckPoint = neckPoint [s];
							neckPoint [s] = neckPoint [s + 1];
							neckPoint [s + 1] = tempNeckPoint;
							swapped = true;
						}
					}
				} while (swapped == true);
				Debug.Log ("done Neck Sort..");

									// list neck Points
				for (int s = 0; s < neckIdx; s++) {
					float dist = Mathf.Sqrt(neckPoint [s].x * neckPoint [s].x + neckPoint [s].z * neckPoint [s].z);
					Debug.Log ("N:  " + s + "  pnt: " + neckPoint [s].x + "," + neckPoint [s].y + "," + neckPoint [s].z + "  Dist: " + dist);
				}
				*/


					writeStartPoints ("startPointsb40.txt");
					//dropping = true;
					//middleOpen = false;

				}
			}
					} // PHASE 1




		if (PHASE == 2) {
			// move batches to lower half
			if (waiting2) {
				waitTime2 = waitTime2 + Time.deltaTime;
				if (waitTime2 > 0.2f) {
					waiting2 = false;
					Debug.Log ("done Waiting2..");

					int neckIdx = (moveBatch - 1) * batchCount;
					int moved = 0;
					float baseY;
					float targetY = 3.5f;
					float shiftY = 0.0f;
					do {
						string dupName = neckPointName [neckIdx + moved];
						GameObject dup = GameObject.Find (dupName);
						if (moved == 0) {
							baseY = dup.transform.position.y;
							shiftY = targetY - baseY;
						}
						Vector3 newPos = new Vector3 (dup.transform.position.x, dup.transform.position.y + shiftY, dup.transform.position.z);
						dup.transform.position = newPos;

						moved = moved + 1;
					} while (moved != batchCount);

					waiting2 = true;
					waitTime2 = 0.0f;
					moveBatch = moveBatch - 1;
					if (moveBatch == 0) {
						waiting2 = false;
						waiting3 = true;
						waitTime3 = 0.0f;

						targetBatch = 80;

						//copyBatch = batches; 
					}

				}
			}

			// create batches in lower half
			if (waiting3) {
				waitTime3 = waitTime3 + Time.deltaTime;
				if (waitTime3 > 0.25f) {
					waiting3 = false;
					//Debug.Log ("done Waiting3..");
					//getBasePoints ();

					float targetY = 3.5f;

					// generate batchCount random Point
					float maxRad = 4.0f;
					float maxY = 3.0f;
					Vector3[] randPoint = new Vector3[batchCount];
					for (int r = 0; r < batchCount; r++) {
						float rad = Random.Range (0.0f, maxRad);
						float angle = Random.Range (0.0f, 360.0f);
						float y = Random.Range (targetY, targetY + maxY);
						Vector2 cartCoords = polarToCartesian (rad, angle * Mathf.Deg2Rad);
						Vector3 pnt = new Vector3(cartCoords.x, y, cartCoords.y);
						randPoint [r] = pnt;
					}

					int copied = 0;
					batches = batches + 1;
					Debug.Log ("done Waiting3.. batches = " + batches);
					currentBatch = batches;
					int idx = 0;

					do {
						duplicates [(currentBatch - 1) * batchCount + idx] = Instantiate (gosg);
						string name = "sandGrain_B" + currentBatch + "_" + (idx + 1);
						duplicates [(currentBatch - 1) * batchCount + idx].name = name;
						GameObject copyDup = GameObject.Find (name);
						copyDup.transform.position = randPoint[idx];
						idx = idx + 1;
						copied = copied + 1;
					} while (copied != batchCount);

					if (batches == targetBatch) {
						waiting3 = false;
						waiting4 = true;
						waitTime4 = 0.0f;
					
					} else 
						waiting3 = true;
					waitTime3 = 0.0f;

				}
			}

			// wait for grains to settle
			if (waiting4) {
				waitTime4 = waitTime4 + Time.deltaTime;
				if (waitTime4 > 1.0f) {
					waiting4 = false;
					Debug.Log ("done Waiting4.. ");
					processing = true;
				}
			}

			if (processing) {



				getBasePoints ();


				int neckPnts = origBatches * batchCount;
				int basePnts = batches * batchCount;

				float minY, maxY, yRange;
				minY = neckPoint [0].y;
				maxY = basePoint [basePnts - 1].y;
				yRange = maxY - minY;
				Debug.Log ("minY: " + minY + "  maxY: " + maxY + "  yRange: " + yRange);

				float lastBand = minY;
				float bandWidth = yRange / bands;


				for (int b = 0; b < bands; b++) {
					bandRange [b].x = lastBand; 
					bandRange [b].y = lastBand + bandWidth;
					bandItems [b] = 0;
					bandRadius[b] = -1f;
					Debug.Log ("Band: " + b + "  lower: " + bandRange [b].x + "  upper: " + bandRange [b].y);

					lastBand = lastBand + bandWidth;
				}


				// add neck band data
				int currentBand = 0;
				int inBand = 0;
				float maxRadius = -1;
				for (int s = 0; s < neckPnts; s++) {
					float radius = Mathf.Sqrt ((neckPoint [s].x * neckPoint [s].x) + (neckPoint [s].z * neckPoint [s].z));
					float yDist = neckPoint [s].y - middleY;
					float y = neckPoint [s].y;

					if (y < bandRange [currentBand].y && y >= bandRange [currentBand].x) { // top value
						if (radius > maxRadius)
							maxRadius = radius;
						inBand = inBand + 1;
					} else {
						Debug.Log ("NECK currentBand: " + currentBand + "  inBand: " + inBand + "  maxRadius: " + maxRadius);

						bandItems [currentBand] = inBand;
						bandRadius [currentBand] = maxRadius;

						inBand = 0;
						maxRadius = -1;
						currentBand = currentBand + 1;

						if (currentBand == bands) {
							Debug.Log ("ERROR s = " + s);
							break;
						}
					}
				}




				// add base band data
				currentBand = bands - 1;
				inBand = 0;
				maxRadius = -1;
				for (int s = basePnts - 1 ; s >= 0; s--) {
					float radius = Mathf.Sqrt ((basePoint [s].x * basePoint [s].x) + (basePoint [s].z * basePoint [s].z));
					float yDist = basePoint [s].y - middleY;
					float y = basePoint [s].y;

					if (y <= bandRange [currentBand].y && y > bandRange [currentBand].x) { // top value
						if (radius > maxRadius)
							maxRadius = radius;
						inBand = inBand + 1;
					} else {
						Debug.Log ("BASE currentBand: " + currentBand + "  inBand: " + inBand + "  maxRadius: " + maxRadius);
					 
						if (bandItems [currentBand] == 0) {
							bandItems [currentBand] = inBand;
							bandRadius [currentBand] = maxRadius;
						} else {
							Debug.Log ("bandItems <> 0");
							if (maxRadius > bandRadius [currentBand]) {
								bandItems [currentBand] = inBand;
								bandRadius [currentBand] = maxRadius;
							}
						}
						inBand = 0;
						maxRadius = -1;
						currentBand = currentBand - 1;

						if (currentBand < 0) {
							Debug.Log ("ERROR s = " + s);
							break;
						}
					}
				}


				for (int b = 0; b < bands; b++) 
					Debug.Log ("Band: " + b + "  RADIUS: " + bandRadius[b] + "  Items: " + bandItems[b] + "  L: " + bandRange [b].x + "  U: " + bandRange [b].y);



				writeBandData ("bandData.txt", bandRange, bandRadius);
				writeBandScript ("bandLine.scr", bandRange, bandRadius);



				processing = false;

				//getNeckBands(middleY);
			}
		} // phase2






    }



	void buildOuterProfile(string profileName, float scaleFactor) {
		GameObject profile_inner, instance, profile_outer;
		Rigidbody profile_outer_RB;

		profile_inner = GameObject.Find (profileName);
		instance = Instantiate (profile_inner);
		instance.transform.parent = hourGlassTR;

		instance.name = "profile_outer";
		instance.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		instance.transform.localScale = new Vector3 (6666.6666f * scaleFactor, 100000.0f * scaleFactor, 6666.6666f * scaleFactor);
		instance.GetComponent<MeshRenderer> ().enabled = false;
		//instance_MR = instance.GetComponent<MeshRenderer> ();
		//instance_MR.enabled = false;
		instance.AddComponent (typeof(Rigidbody));

		profile_outer = GameObject.Find ("profile_outer");
		profile_outer_RB = profile_outer.GetComponent<Rigidbody> ();
		profile_outer_RB.mass = 100000;
		profile_outer_RB.isKinematic = true;
		profile_outer_RB.useGravity = false;
	}


	void setupProfile() {

		Transform    profile_A_top_TR = null, profile_A_bot_TR = null, profile_B_top_TR = null, profile_B_bot_TR = null, profile_C_top_TR = null, profile_C_bot_TR = null;
		MeshRenderer profile_A_top_MR = null, profile_A_bot_MR = null, profile_B_top_MR = null, profile_B_bot_MR = null, profile_C_top_MR = null, profile_C_bot_MR = null;
		MeshCollider profile_A_top_MC = null, profile_A_bot_MC = null, profile_B_top_MC = null, profile_B_bot_MC = null, profile_C_top_MC = null, profile_C_bot_MC = null;

		GameObject GO;

		// profiles
		GO = GameObject.Find ("profile_A_top");
		if (GO == null)
			Debug.Log ("NOT GOT profile_A_top");
		else {
			profile_A_top_TR = GO.GetComponent<Transform> ();
			profile_A_top_MR = GO.GetComponent<MeshRenderer> ();
			profile_A_top_MC = GO.GetComponent<MeshCollider> ();
			Debug.Log ("GOT profile_A_top");
		}
		GO = GameObject.Find ("profile_A_bot");
		if (GO == null)
			Debug.Log ("NOT GOT profile_A_bot");
		else {
			profile_A_bot_TR = GO.GetComponent<Transform> ();
			profile_A_bot_MR = GO.GetComponent<MeshRenderer> ();
			profile_A_bot_MC = GO.GetComponent<MeshCollider> ();
			Debug.Log ("GOT profile_A_bot");
		}

		//GO = GameObject.Find ("profile_B_top");
		//if (GO == null)
		//	Debug.Log ("NOT GOT profile_B_top");
		//else {
		//	profile_B_top_TR = GO.GetComponent<Transform> ();
		//	profile_B_top_MR = GO.GetComponent<MeshRenderer> ();
		//	profile_B_top_MC = GO.GetComponent<MeshCollider> ();
		//	Debug.Log ("GOT profile_B_top");
		//}
		//GO = GameObject.Find ("profile_B_bot");
		//if (GO == null)
		//	Debug.Log ("NOT GOT profile_B_bot");
		//else {
		//	profile_B_bot_TR = GO.GetComponent<Transform> ();
		//	profile_B_bot_MR = GO.GetComponent<MeshRenderer> ();
		//	profile_B_bot_MC = GO.GetComponent<MeshCollider> ();
		//	Debug.Log ("GOT profile_B_bot");
		//}
		//GO = GameObject.Find ("profile_C_top");
		//if (GO == null)
		//	Debug.Log ("NOT GOT profile_C_top");
		//else {
		//	profile_C_top_TR = GO.GetComponent<Transform> ();
		//	profile_C_top_MR = GO.GetComponent<MeshRenderer> ();
		//	profile_C_top_MC = GO.GetComponent<MeshCollider> ();
		//	Debug.Log ("GOT profile_C_top");
		//}
		//GO = GameObject.Find ("profile_C_bot");
		//if (GO == null)
		//	Debug.Log ("NOT GOT profile_C_bot");
		//else {
		//	profile_C_bot_TR = GO.GetComponent<Transform> ();
		//	profile_C_bot_MR = GO.GetComponent<MeshRenderer> ();
		//	profile_C_bot_MC = GO.GetComponent<MeshCollider> ();
		//	Debug.Log ("GOT profile_C_bot");
		//}

		if (profile == PROFILE_A) {
			profile_A_top_MR.enabled = true;
			profile_A_top_MC.enabled = true;
			profile_A_bot_MR.enabled = true;
			profile_A_bot_MC.enabled = true;

			//profile_B_top_MR.enabled = false;
			//profile_B_top_MC.enabled = false;
			//profile_B_bot_MR.enabled = false;
			//profile_B_bot_MC.enabled = false;

			//profile_C_top_MR.enabled = false;
			//profile_C_top_MC.enabled = false;
			//profile_C_bot_MR.enabled = false;
			//profile_C_bot_MC.enabled = false;

			buildOuterProfile ("profile_A_top", 1.01f);

		} 
        /*
        else if (profile == PROFILE_B) {
			profile_A_top_MR.enabled = false;
			profile_A_top_MC.enabled = false;
			profile_A_bot_MR.enabled = false;
			profile_A_bot_MC.enabled = false;

			profile_B_top_MR.enabled = true;
			profile_B_top_MC.enabled = true;
			profile_B_bot_MR.enabled = true;
			profile_B_bot_MC.enabled = true;

			profile_C_top_MR.enabled = false;
			profile_C_top_MC.enabled = false;
			profile_C_bot_MR.enabled = false;
			profile_C_bot_MC.enabled = false;

			buildOuterProfile ("profile_B_top", 1.01f);

		} else if (profile == PROFILE_C) {
			profile_A_top_MR.enabled = false;
			profile_A_top_MC.enabled = false;
			profile_A_bot_MR.enabled = false;
			profile_A_bot_MC.enabled = false;

			profile_B_top_MR.enabled = false;
			profile_B_top_MC.enabled = false;
			profile_B_bot_MR.enabled = false;
			profile_B_bot_MC.enabled = false;

			profile_C_top_MR.enabled = true;
			profile_C_top_MC.enabled = true;
			profile_C_bot_MR.enabled = true;
			profile_C_bot_MC.enabled = true;

			buildOuterProfile ("profile_C_top", 1.01f);
		} 
        */

	}







	void calibrateA(string startPointsFilename, int setBatches, float turnSpeed, int calibrationTurns, float calibrationSpeed, string dataTextFilename, string dataScriptFilename) {

		// calibrate Method A
		// read startPoint (in upper chamber)
		// wait - then determine calibrateNeckDist
		// rotate - 180
		// wait - then determine calibrateBaseDist and band data
		// rotate 180 at slow speed and processCalibrationPoints (measure each point - determine band and radius and adjust maxRadius for each band) - then wait
		// repeat above calibrateTargetTurns

		if (calibrateReadPoints) {
			readStartPoints (startPointsFilename, 0, setBatches, 4.0f);
			batches = setBatches;
			calibrateReadPoints = false;
			calibrateWaiting = true;
			Debug.Log ("done readg...");
		}

		if (calibrateWaiting) {
			calibrateWaitTime = calibrateWaitTime + Time.deltaTime;
			if (calibrateWaitTime > 3.0f) {
				calibrateWaiting = false;
				calibrateMeasureNeck = true;
				Debug.Log ("done calibrateWaiting...");
				calibrateNeckDist = 1000000; 
			}
		}

		if (calibrateMeasureNeck) {
			for (int b = 1; b <= batches; b++) {
				for (int d = 1; d <= batchCount; d++) {
					string name = "sandGrain_B" + b + "_" + d;
					GameObject dup = GameObject.Find (name);
					float neckDist = Mathf.Abs (dup.transform.position.y - middleY);
					if (neckDist < calibrateNeckDist)
						calibrateNeckDist = neckDist; 
				}
			}
			calibrateNeckDist = calibrateNeckDist + middleY; 
			Debug.Log ("middleY: " + middleY);
			Debug.Log ("calibrateNeckDist: " + calibrateNeckDist);
			Debug.Log ("done calibrateMeasureNeck...");
			calibrateMeasureNeck = false;

			calibrateRotating = true;
			calibrateRotated = 0.0f;
			calibrateRotateSpeed = turnSpeed; // deg / sec
		}

		if (calibrateRotating) {
			float rot = calibrateRotateSpeed * Time.deltaTime;
			calibrateRotated = calibrateRotated + rot; 
			if (calibrateRotated >= 180f) {
				calibrateRotating = false;
				calibrateWaiting2 = true;
				Debug.Log ("done calibrateRotating..");
				rot = rot - (calibrateRotated - 180);
			}
			Debug.Log ("calibrateRotated: " + calibrateRotated);
			hourGlassTR.Rotate (rot, 0, 0);
		}

		if (calibrateWaiting2) {
			calibrateWaitTime2 = calibrateWaitTime2 + Time.deltaTime;
			if (calibrateWaitTime2 > 2.0f) {
				calibrateWaiting2 = false;
				calibrateMeasureBase = true;
				Debug.Log ("done calibrateWaiting2...");
				calibrateBaseDist = -1; 
			}
		}

		if (calibrateMeasureBase) {
			for (int b = 1; b <= batches; b++) {
				for (int d = 1; d <= batchCount; d++) {
					string name = "sandGrain_B" + b + "_" + d;
					GameObject dup = GameObject.Find (name);
					float baseDist = Mathf.Abs (dup.transform.position.y - middleY);
					if (baseDist > calibrateBaseDist)
						calibrateBaseDist = baseDist; 
				}
			}
			calibrateBaseDist = calibrateBaseDist + middleY; 
			Debug.Log ("middleY: " + middleY);
			Debug.Log ("calibrateBaseDist: " + calibrateBaseDist);
			Debug.Log ("done calibrateMeasureBase...");

			float range = calibrateBaseDist - calibrateNeckDist;
			float bandWidth = range / bands;
			float lastBand = calibrateNeckDist;

			for (int b = 0; b < bands; b++) {
				calibrateRange [b].x = lastBand; 
				calibrateRange [b].y = lastBand + bandWidth;
				calibrateRadius [b] = 0;
				lastBand = lastBand + bandWidth;
			}

			calibrateMeasureBase = false;

			calibrateRotating2 = true;
			calibrateRotated2 = 0.0f;
			calibrateRotateSpeed = calibrationSpeed; // deg / sec
			calibrateTurns = 0;
			calibrateTargetTurns = calibrationTurns;
		}


		if (calibrateRotating2) {
			float rot = calibrateRotateSpeed * Time.deltaTime;
			calibrateRotated2 = calibrateRotated2 + rot; 
			if (calibrateRotated2 >= 180f) {
				calibrateRotating2 = false;
				calibrateTurns = calibrateTurns + 1;

				calibrateWaiting3 = true;
				Debug.Log ("TURN: " + calibrateTurns + " done calibrateRotating2..");
				rot = rot - (calibrateRotated2 - 180);
			}
			Debug.Log ("calibrateRotated2: " + calibrateRotated2);
			processCalibrationPoints ();
			hourGlassTR.Rotate (rot, 0, 0);
		}

		if (calibrateWaiting3) {
			calibrateWaitTime3 = calibrateWaitTime3 + Time.deltaTime;
			processCalibrationPoints ();
			if (calibrateWaitTime3 > 2.0f) {
				calibrateWaiting3 = false;

				if (calibrateTurns < calibrateTargetTurns) {
					calibrateRotating2 = true;
					calibrateRotated2 = 0.0f;
					calibrateRotateSpeed = calibrationSpeed; // deg / sec
				} else {
					calibrateRotating2Finished = true;
				}
			}
		}

		if (calibrateRotating2Finished) {
			calibrateRotating2Finished = false;
			writeBandData (dataTextFilename, calibrateRange, calibrateRadius);
			writeBandScript (dataScriptFilename, calibrateRange, calibrateRadius);
			Debug.Log ("Done calibrating...");
		}
	}












	void shakeAllGrains(float shake) {
		Vector3 randVector;
		float shakeDiv2 = shake / 2;
		string name;
		Vector3 grainPoint;
		GameObject dup;

		for (int b = 1; b <= batches; b++) {
			for (int d = 1; d <= batchCount; d++) {
				name = "sandGrain_B" + b + "_" + d;
				dup = GameObject.Find (name);
				grainPoint = dup.transform.position;
				randVector = new Vector3 (Random.Range (-shakeDiv2, shakeDiv2), Random.Range (-shakeDiv2, shakeDiv2), Random.Range (-shakeDiv2, shakeDiv2));
				grainPoint = grainPoint + randVector;
				dup.transform.position = grainPoint;
			}
		}
	}






	void updateCalibrateBandRadius(float lookup, float radius) {
		float maxRadius = -1;;
		for (int b = 0; b < bands; b++) {
			if (b == 0) {
				if (lookup < calibrateRange [b].y) 
					maxRadius = calibrateRadius [b];
			} else if (b == bands - 1) {
				if (lookup >= calibrateRange [b].x)
					maxRadius = calibrateRadius [b];
			} else {
				if (lookup >= calibrateRange [b].x && lookup <= calibrateRange [b].y) 
					maxRadius = calibrateRadius [b];
			}
			if (maxRadius != -1) {
				//Debug.Log ("maxRadius: " + maxRadius);
				if (radius > maxRadius) {
					calibrateRadius [b] = radius;
				}
				break;
			}
		}

	}

	void processCalibrationPoints() {
		Vector3 middlePoint = middlePlugTR.transform.position;
		Vector3 basePoint = counterBottomTR.transform.position;
		string name;
		GameObject dup;
		Vector3 grainPoint, pntOnLine;
		float radius, lookup;

		for (int b = 1; b <= batches; b++) {
			for (int d = 1; d <= batchCount; d++) {
				name = "sandGrain_B" + b + "_" + d;
				dup = GameObject.Find (name);
				grainPoint = dup.transform.position;

				pntOnLine = nearestPointOnLine (middlePoint, basePoint, grainPoint);
				radius = Vector3.Distance (grainPoint, pntOnLine);
				lookup = Vector3.Distance (middlePoint, pntOnLine) + middleY;

				//Debug.Log ("B: " + b + "  D: " + d + "  R: " + radius + "  L: " + lookup);




				updateCalibrateBandRadius (lookup, radius);



			}
		}
	}

	float doPushBack(string label, bool debug, bool checkDropped, bool getMaxSpeed) {
		Vector3 middlePoint = middlePlugTR.transform.position;
		Vector3 basePoint = counterBottomTR.transform.position;
		Vector3 middleVector = middlePoint - basePoint;
		float middleDistance = Vector3.Distance (middlePoint, basePoint);

		float radius, lookup, fraction, maxRad;
		string name;
		bool outOfRange, pushedBack;
		Vector3 grainPoint, pntOnLine, newPnt;
		GameObject dup;

		float maxSpeed = -1.0f;
			
		string errorStr = "PUSHED: ";
		int rCount = 0;
		int bCount = 0;
		int lCount = 0;
		for (int b = 1; b <= batches; b++) {
			for (int d = 1; d <= batchCount; d++) {

				int idx = (b - 1) * batchCount + d - 1;
				bool carryOn = true;;
				if (checkDropped && !grainDropped [idx])
					carryOn = false;
				if (carryOn) {

					name = "sandGrain_B" + b + "_" + d;
					dup = GameObject.Find (name);

					if (getMaxSpeed) {
						Vector3 velocity = dup.GetComponent<Rigidbody> ().velocity;
						float speed = Mathf.Sqrt (velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);
						if (speed > maxSpeed)
							maxSpeed = speed;
					}

					grainPoint = dup.transform.position;
					pntOnLine = nearestPointOnLine (middlePoint, basePoint, grainPoint);
					radius = Vector3.Distance (grainPoint, pntOnLine);
					lookup = Vector3.Distance (middlePoint, pntOnLine) + middleY;

					outOfRange = false;
					//float tolerance = 0.1f;
					if (lookup < bandRange [0].x || lookup > bandRange [bands - 1].y) {
						outOfRange = true;
						if (lookup > bandRange [bands - 1].y) {
							float pushUp = lookup - bandRange [bands - 1].y;
							fraction = pushUp / middleDistance;
							newPnt = dup.transform.position + fraction * middleVector;
							dup.transform.position = newPnt;

							grainPoint = newPnt;
							pntOnLine = nearestPointOnLine (middlePoint, basePoint, grainPoint);
							radius = Vector3.Distance (grainPoint, pntOnLine);
							lookup = Vector3.Distance (middlePoint, pntOnLine) + middleY;

						} else {
							lCount = lCount + 1;
							//Debug.Log ("lookup out of range[0] LOWER");
						}
					}


					pushedBack = false;
					maxRad = getBandRadius (lookup);
					if (radius > maxRad + maxBulge) {
						pushedBack = true;

						Vector3 normalVector = grainPoint - pntOnLine;
						fraction = maxRad / radius;
						newPnt = pntOnLine + fraction * normalVector;

						float testDist = Vector3.Distance (newPnt, pntOnLine);
						float fuzz = 0.00001f;
						if (testDist > maxRad + fuzz)
							Debug.Log ("======================      testDist > maxRad + fuzz");

						dup.transform.position = newPnt;
					}

					if (pushedBack) {
						rCount = rCount + 1;
						errorStr = errorStr + "<" + b + "-" + d;
						if (outOfRange)
							errorStr = errorStr + ">";
					}
					if (outOfRange && !pushedBack) {
						bCount = bCount + 1;
						errorStr = errorStr + b + "==" + d;
					}
					if (outOfRange || pushedBack)
						errorStr = errorStr + " ";
				}
			}
		}
		if (!errorStr.Equals ("PUSHED: "))
			Debug.Log (errorStr);
	

		if ((rCount > 0 || bCount > 0 || lCount > 0) && debug)
			Debug.Log (label + "  PUSHED: rCount: " + rCount + "  bCount: " + bCount + "  lCount: " + lCount);

		return maxSpeed;

	}





	public static Vector2 polarToCartesian(float rad, float ang) {
		Vector2 res;
		res.x = rad * Mathf.Cos(ang);
		res.y = rad * Mathf.Sin(ang);
		return res; // (x,y)
	}

float getBandRadius(float lookup) {
	float maxRadius = -1;;
	for (int b = 0; b < bands; b++) {
		if (b == 0) {
			if (lookup < bandRange [b].y) 
				maxRadius = bandRadius [b];
		} else if (b == bands - 1) {
			if (lookup >= bandRange [b].x)
				maxRadius = bandRadius [b];
		} else {
			if (lookup >= bandRange [b].x && lookup <= bandRange [b].y) 
				maxRadius = bandRadius [b];
		}
		if (maxRadius != -1)
			break;
	}
	return maxRadius;
}

	void writeBandData(string fileName, Vector2[] writeRange, float[] writeRadius) {
		string path = "Assets/calibrationData/" + fileName;
		string bStr;
		using (StreamWriter sw = new StreamWriter (path)) {
			for (int b = 0; b < bands; b++) {
				bStr = b.ToString ();
				if (b < 10)
					bStr = "00" + bStr;
				if (b < 100)
					bStr = "0" + bStr;

				sw.WriteLine ("B: " + bStr + " " + writeRadius[b].ToString ("F6") + " " + 
					writeRange[b].x.ToString ("F6") + " " + writeRange[b].y.ToString ("F6"));
			}
		}
		Debug.Log ("bandData Written " + fileName);
	}

	void writeBandScript(string fileName, Vector2[] writeRange, float[] writeRadius) {
		string path = "Assets/calibrationData/" + fileName;
		using (StreamWriter sw = new StreamWriter (path)) {
			sw.WriteLine ("LINE");
			for (int b = 0; b < bands; b++) 
				sw.WriteLine (writeRadius [b].ToString ("F6") + "," + writeRange [b].x.ToString ("F6"));
			sw.WriteLine ("");
		}
		Debug.Log ("bandData Written " + fileName);
	}

	void writeStartPoints(string fileName) {
		string path = "Assets/" + fileName;
		string name, bStr, dStr, posStr, rotStr;

		using (StreamWriter sw = new StreamWriter (path)) {
			for (int b = 1; b <= batches; b++) {
				for (int d = 1; d <= batchCount; d++) {
					name = "sandGrain_B" + b + "_" + d;
					GameObject dup = GameObject.Find (name);
					bStr = b.ToString ();
					if (b < 10)
						bStr = "0" + bStr;

					dStr = d.ToString ();
					if (d < 10)
						dStr = "00" + dStr;
					else if (d < 100)
						dStr = "0" + dStr;

					posStr = "";
					if (dup.transform.position.x >= 0)
						posStr = posStr + " ";
					posStr = posStr + dup.transform.position.x.ToString ("F6");
					posStr = posStr + ",";
					if (dup.transform.position.y >= 0)
						posStr = posStr + " ";
					posStr = posStr + dup.transform.position.y.ToString ("F6");
					posStr = posStr + ",";
					if (dup.transform.position.z >= 0)
						posStr = posStr + " ";
					posStr = posStr + dup.transform.position.z.ToString ("F6");

					rotStr = "";
					if (dup.transform.rotation.x >= 0)
						rotStr = rotStr + " ";
					rotStr = rotStr + dup.transform.rotation.x.ToString ("F6");
					rotStr = rotStr + ",";
					if (dup.transform.rotation.y >= 0)
						rotStr = rotStr + " ";
					rotStr = rotStr + dup.transform.rotation.y.ToString ("F6");
					rotStr = rotStr + ",";
					if (dup.transform.rotation.z >= 0)
						rotStr = rotStr + " ";
					rotStr = rotStr + dup.transform.rotation.z.ToString ("F6");

					sw.WriteLine (bStr + " " + dStr + " " + posStr + " " + rotStr, sw);
					//writeLine (p1.x.ToString ("F6") + "," + p1.y.ToString ("F6"), sw);
					//writeLine (p2.x.ToString ("F6") + "," + p2.y.ToString ("F6"), sw);
					//writeLine ("", sw);
				}
			}
		}
		Debug.Log ("startPoints Written " + fileName);
	}

	void readStartPoints(string fileName, int previousBatches, int fileBatches, float lift) {
		string path = "Assets/pointsData/" + fileName;
		string dataStr;
		string bStr, dStr, posStr, rotStr;
		int b, d;

		using (StreamReader sr = new StreamReader (path)) {
			for (int i = 0; i < fileBatches * batchCount; i++) {
				dataStr = sr.ReadLine ();
				bStr = dataStr.Substring (0, 2);
				dStr = dataStr.Substring (3, 3);

				posStr = dataStr.Substring (7, 30);
				rotStr = dataStr.Substring (38);
				string[] posStrings = posStr.Split (',');
				float[] pos = new float[posStrings.Length];
				for (int j = 0; j < pos.Length; j++)
					pos [j] = float.Parse (posStrings [j].Trim ());
				string[] rotStrings = rotStr.Split (',');
				float[] rot = new float[rotStrings.Length];
				for (int j = 0; j < rot.Length; j++)
					rot [j] = float.Parse (rotStrings [j].Trim ());


				// LIFT all for calibrate new shape
				pos [1] = pos [1] + lift;




				b = int.Parse (bStr.Trim ());
				d = int.Parse (dStr.Trim ());
				Vector3 position = new Vector3 (pos [0], pos [1], pos [2]);
				Vector3 rotation = new Vector3 (rot [0], rot [1], rot [2]);

				///Debug.Log ("B: " + b + "  D:" + d +
				//	"  POS: " + position.x + "," + position.y + "," + position.z +
				//	"  ROT: " + rotation.x + "," + rotation.y + "," + rotation.z);
				b = b + previousBatches; 
				int index = previousBatches * batchCount + i;
				duplicates [index] = Instantiate (gosg);
				grainDropped [index] = false;
				string dName = "sandGrain_B" + b + "_" + d;
				duplicates [index].name = dName;
				GameObject dup = GameObject.Find (dName);
				//dup.transform.parent = middlePlugTR.transform.parent;
				//dup.transform.localScale = new Vector3 (8.0f, 120.0f, 8.0f);
				dup.transform.position = position;
				dup.transform.rotation = Quaternion.Euler (rot[0], rot[1], rot[2]);
			}






		}
		Debug.Log ("Read startPoints");
	}




	void readBandData(string fileName, float lastBandTweak, int calibrationPnt1, int calibrationPnt2) {  // calPnt1 > calPnt2 (169 > 6)
		string path = "Assets/calibrationData/" + fileName;
		string dataStr, rStr, lowerStr, upperStr;
		float radius, lower, upper;

		using (StreamReader sr = new StreamReader (path)) {
			for (int i = 0; i < bands; i++) {
				dataStr = sr.ReadLine ();
				rStr = dataStr.Substring (7, 8);
				radius = float.Parse (rStr);
				lowerStr = dataStr.Substring (16, 9);
				lower = float.Parse (lowerStr);
				upperStr = dataStr.Substring (26, 9);
				upper = float.Parse (upperStr);

				if (i == bands - 1) 
					upper = upper - baseBandReduction;
				bandRadius [i] = radius;
				bandRange [i].x = lower;	
				bandRange [i].y = upper;	

				Debug.Log ("Band: " + i + "  Rad: " + bandRadius [i] + "  Lower: " + bandRange [i].x + "  Upper: " + bandRange [i].y);
			}
		}

		// tweak last Band
		bandRange [bands - 1].y = bandRange [bands - 1].y + lastBandTweak;   //   (- 0.005f)


		// re-calibrate between 0 and 169 - using band 6 and 169
        if (calibrationPnt1 != -1 && calibrationPnt2 != -1) {
		    int calPnt1 = calibrationPnt1;
		    int calPnt2 = calibrationPnt2;
		    int calBands = calPnt1 - calPnt2;
		    float rad1 = bandRadius [calPnt1];
		    float rad2 = bandRadius [calPnt2];
		    float deltaRadius = (rad1 - rad2) / calBands;
		    Debug.Log ("deltaRadius = " + deltaRadius);
		    float rad = rad1;
		    for (int i = calPnt1 - 1; i >= 0; i--) {
			    rad = rad - deltaRadius;
			    bandRadius[i] = rad;
		    }
		    Debug.Log ("bandRadius[" + calPnt1 + "] = " + bandRadius [calPnt1]);
		    Debug.Log ("bandRadius[" + calPnt2 + "] = " + bandRadius [calPnt2]);
        }
        
        
		Debug.Log ("Read bandData");
	}

	void getNeckPoints() {
		int neckIdx = 0;
		Vector3 tempNeckPoint;
		string tempName;
		for (int b = 1; b <= batches; b++) {
			for (int d = 1; d <= batchCount; d++) {
				string name = "sandGrain_B" + b + "_" + d;
				GameObject dup = GameObject.Find (name);
				//Debug.Log ("NECK: " + name);
				neckPoint [neckIdx] = dup.transform.position;
				neckPointName [neckIdx] = name;
				neckIdx = neckIdx + 1;
			}
		}
		// sort neck Points
		bool swapped;
		do {
			swapped = false;
			for (int s = 0; s < neckIdx - 1; s++) {
				if (neckPoint [s].y > neckPoint [s + 1].y) { // swap 
					tempNeckPoint = neckPoint [s];
					tempName = neckPointName [s];

					neckPoint [s] = neckPoint [s + 1];
					neckPointName [s] = neckPointName [s + 1];
					neckPoint [s + 1] = tempNeckPoint;
					neckPointName [s + 1] = tempName;
					swapped = true;
				}
			}
		} while (swapped == true);
		// list neck Points
		//for (int s = 0; s < neckIdx; s++) {
		//	float dist = Mathf.Sqrt(neckPoint [s].x * neckPoint [s].x + neckPoint [s].z * neckPoint [s].z);
		//	Debug.Log ("NECK:  " + s + " " + neckPointName[s] + "  pnt: " + neckPoint [s].x + "," + neckPoint [s].y + "," + neckPoint [s].z + "  Dist: " + dist);
		//}
		Debug.Log ("done Neck Sort..");
	}

	void getNeckBands(float middleY) {
		int neckIdx = batches * batchCount; 
		float bandWidth = 0.1f;
		float bandStart = 0f;
		int bandCount = 0;
		float maxRadius = 0;
		int band = 0;
		for (int s = 0; s < neckIdx; s++) {
			float radius = Mathf.Sqrt ((neckPoint [s].x * neckPoint [s].x) + (neckPoint [s].z * neckPoint [s].z));
			float yDist = neckPoint [s].y - middleY;
			if (s == 0)
				bandStart = neckPoint [s].y - middleY; // lowest y
			float bandDist = yDist - bandStart;
			//Debug.Log ("S: " + s + "  radius: " + radius + "  yDist: " + yDist + "  bandDist: " + bandDist);
			if (bandDist < bandWidth) {
				if (radius > maxRadius)
					maxRadius = radius;
				bandCount = bandCount + 1;
			} else {
				Debug.Log ("Band: " + band + "  bandCount: " + bandCount + "  maxRadius: " + maxRadius);
				bandCount = 0;
				maxRadius = 0;
				bandStart = bandStart + bandWidth;
				band = band + 1;
			}
		}
	}

	void getBasePoints() {
		int baseIdx = 0;
		Vector3 tempBasePoint;
		string tempName;
		for (int b = 1; b <= batches; b++) {
			for (int d = 1; d <= batchCount; d++) {
				string name = "sandGrain_B" + b + "_" + d;
				GameObject dup = GameObject.Find (name);
				//Debug.Log ("NECK: " + name);
				// mirror points on mid line
				float midDist = middleY - dup.transform.position.y;
				basePoint [baseIdx] = dup.transform.position;
				basePoint [baseIdx].y = basePoint [baseIdx].y + 2 * midDist;
				basePointName [baseIdx] = name;
				baseIdx = baseIdx + 1;
			}
		}
		Debug.Log ("baseIdx: " + baseIdx);
		// sort base Points
		bool swapped;
		do {
			swapped = false;
			for (int s = 0; s < baseIdx - 1; s++) {
				if (basePoint [s].y > basePoint [s + 1].y) { // swap 
					tempBasePoint = basePoint [s];
					tempName = basePointName [s];

					basePoint [s] = basePoint [s + 1];
					basePointName [s] = basePointName [s + 1];
					basePoint [s + 1] = tempBasePoint;
					basePointName [s + 1] = tempName;
					swapped = true;
				}
			}
		} while (swapped == true);
		Debug.Log ("basePnt[0].y = " + basePoint [0].y);
		Debug.Log ("basePnt[" + (baseIdx - 1) + "].y = " + basePoint [baseIdx - 1].y);
		Debug.Log ("done Base Sort..");
		// list base Points
		//for (int s = 0; s < baseIdx; s++) {
		//	float dist = Mathf.Sqrt(basePoint [s].x * basePoint [s].x + basePoint [s].z * basePoint [s].z);
		//	Debug.Log ("BASE:  " + s + " " + basePointName[s] + "  pnt: " + basePoint [s].x + "," + basePoint [s].y + "," + basePoint [s].z + "  Dist: " + dist);
		//}
	}


	public static Vector3 nearestPointOnLine(Vector3 linePnt1, Vector3 linePnt2, Vector3 pnt) {
		Vector3 lineDir = new Vector3(linePnt2.x - linePnt1.x, linePnt2.y - linePnt1.y, linePnt2.z - linePnt1.z);
		lineDir.Normalize ();  // make unit vector

		Vector3 v = pnt - linePnt1;
		float d = Vector3.Dot (v, lineDir);
		Vector3 res = linePnt1 + lineDir * d;

		//float dist = Vector3.Distance (linePnt1, linePnt2);
		//float dist1 = Vector3.Distance (linePnt1, res);
		//float dist2 = Vector3.Distance (res, linePnt2);
		//Debug.Log ("dist: " + dist + "  dist1 " + dist1 + "  dist2 " + dist2);

		return res;
	}




    /*
	void OnTriggerEnter(Collider col) {
		string colName = col.name;
		Debug.Log ("TRIGGER_ENTER  =====================" + colName);
	}
	void OnTriggerExit(Collider col) {
		string colName = col.name;
		Debug.Log ("TRIGGER_EXIT  =====================" + colName);
	}
	Vector3 worldToHourGlass(Vector3 worldPoint, float tilt) {
		Vector3 pivot = new Vector3(1.0f, 0.0f, 0.0f); // x-axis
		Vector3 angles = new Vector3(tilt, 0.0f, 0.0f);
		Vector3 hourGlassPoint = RotateAroundPivot (worldPoint, pivot, angles);
		return hourGlassPoint;
	}
	Vector3 houGlassToWorld(Vector3 hourGlassPoint, float tilt) {
		Vector3 pivot = new Vector3(1.0f, 0.0f, 0.0f); // x-axis
		Vector3 angles = new Vector3(tilt, 0.0f, 0.0f);
		Vector3 worldPoint = RotateAroundPivot (hourGlassPoint, pivot, angles);
		return worldPoint;
	}
	Vector3 RotateAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles) {
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}
	public static float getDistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
		Vector3 PointThing = new Vector3 (lineStart.x - point.x, lineStart.y - point.y, lineStart.z - point.z);  
		Vector3 TotalThing = new Vector3 (PointThing.y * lineEnd.z - PointThing.z * lineEnd.y,
			                     PointThing.x * lineEnd.z - PointThing.z * lineEnd.x,
			                     PointThing.x * lineEnd.y - PointThing.y * lineEnd.x);
		float dist = Vector3.Distance (lineStart, lineEnd);
		float dist1 = Vector3.Distance (lineStart, TotalThing);
		float dist2 = Vector3.Distance (TotalThing, lineEnd);
		Debug.Log ("dist: " + dist + "  dist1 " + dist1 + "  dist2 " + dist2);
		float distance = (float) (Mathf.Sqrt(TotalThing.x * TotalThing.x + TotalThing.y * TotalThing.y + TotalThing.z * TotalThing.z) /
			Mathf.Sqrt(lineEnd.x * lineEnd.x + lineEnd.y * lineEnd.y + lineEnd.z * lineEnd.z ));
		return distance;
	}
    */

}


	/*
	public static float betweenPointAndLine(float[] point, float[] lineStart, float[] lineEnd){
		float[] PointThing = new float[3];
		float[] TotalThing = new float[3];
		PointThing[0] = lineStart[0] - point[0];
		PointThing[1] = lineStart[1] - point[1];
		PointThing[2] = lineStart[2] - point[2];

		TotalThing[0] = (PointThing[1]*lineEnd[2] - PointThing[2]*lineEnd[1]);
		TotalThing[1] = -(PointThing[0]*lineEnd[2] - PointThing[2]*lineEnd[0]);
		TotalThing[2] = (PointThing[0]*lineEnd[1] - PointThing[1]*lineEnd[0]);

		float distance = (float) (Math.sqrt(TotalThing[0]*TotalThing[0] + TotalThing[1]*TotalThing[1] + TotalThing[2]*TotalThing[2]) /
			Math.sqrt(lineEnd[0] * lineEnd[0] + lineEnd[1] * lineEnd[1] + lineEnd[2] * lineEnd[2] ));


		return distance;
	}
	*/

/*

    if (dropping) {
			if (!middleOpen) {
				allDropped = false;
				middlePlugMR.enabled = false;
				middlePlugMC.enabled = false;
				middleOpen = true;
			}

			int dropped = 0;
			for (int b = 1; b <= batches; b++) {
				for (int d = 0; d < batchCount; d++) {
					int idx = (b - 1) * batchCount + d;
					if (duplicates [idx].transform.position.y < 13.0f)
						dropped = dropped + 1;
				}
			}
			Debug.Log ("dropped = " + dropped);
			if (dropped == batches * batchCount) {
				dropping = false;
				waiting2 = true;
				waitTime2 = 0.0f;
				allDropped = true;
				Debug.Log ("set Waiting2..");
			}
		}

		if (waiting2) {
			waitTime2 = waitTime2 + Time.deltaTime;
			if (waitTime2 > 4.0f) {
				waiting2 = false;
				Debug.Log ("done Waiting2..");

				// get base Points
				float middleZ = middlePlugTR.transform.worldPosition.z;
				Debug.Log ("middleZ: " + middleZ);

				int baseIdx = 0;
				Vector3[] basePoint = new Vector3[batchCount * maxBatch];
				Vector3 tempBasePoint;
				for (int b = 1; b <= batches; b++) {
					for (int d = 1; d <= batchCount; d++) {
						string name = "sandGrain_B" + b + "_" + d;
						GameObject dup = GameObject.Find (name);
						if (dup.transform.position.y < middleZ) {
							Debug.Log ("BASE: " + name);
							basePoint [baseIdx] = dup.transform.position;
							baseIdx = baseIdx + 1;
						}
					}
				}

				// sort base Points
				bool swapped;
				do {
					swapped = false;
					for (int s = 0; s < baseIdx - 1; s++) {
						if (basePoint [s].y > basePoint [s + 1].y) { // swap 
							tempBasePoint = basePoint [s];
							basePoint [s] = basePoint [s + 1];
							basePoint [s + 1] = tempBasePoint;
							swapped = true;
						}
					}
				} while (swapped == true);
				Debug.Log ("done Base Sort..");

				// list base Points
				for (int s = 0; s < baseIdx; s++) {
					float dist = Mathf.Sqrt(basePoint [s].x * basePoint [s].x + basePoint [s].z * basePoint [s].z);
					Debug.Log ("B:  " + s + "  pnt: " + basePoint [s].x + "," + basePoint [s].y + "," + basePoint [s].z + "  Dist: " + dist);
				}


				//rotatingHourGlass = true;
				middlePlugMR.enabled = true;
				middlePlugMC.enabled = true;



				//for (int b = 1; b <= batches; b++) {
				//	for (int d = 1; d <= batchCount; d++) {
				//		string name = "sandGrain_B" + b + "_" + d;
				//		GameObject dup = GameObject.Find (name);
				//		Collider mc = dup.GetComponent<Collider> ();
				//		mc.isTrigger = true;
				//	}
				//}



			}
		}


		//if (doRotation && !rotatingHourGlass) {
		//middlePlugMR.enabled = true;
		//middlePlugMC.enabled = true;

			//rotatingHourGlass = true;
			//rotated = 0.0f; 
			//rotateSpeed = 0.0f; // deg / sec

			//hourGlassRotation = hourGlassRotation + rotateSpeed * Time.deltaTime;
			//middlePlugTR.transform.rotation = Quaternion.Euler (hourGlassRotation, 0.0f, 0.0f);


		//}



		if (rotatingHourGlass) {
			float rotateSpeedIncrement = 4.0f; // deg / sec
			rotateSpeed = rotateSpeed + rotateSpeedIncrement * Time.deltaTime; // deg / sec
			if (rotateSpeed > 13)
				rotateSpeed = 13.0f;
			Debug.Log ("rotateSpeed: " + rotateSpeed);
			float rot = rotateSpeed * Time.deltaTime;
			rotated = rotated + rot; 
			if (rotated >= 180.0f) {
				rotatingHourGlass = false;

				//rotateFinished = true;
				//rotateFinished = false;
				waiting3 = true;
				waitTime3 = 0.0f;
				Debug.Log ("done rotating..");

				if (rotated > 180.0f)
					rot = rot - (rotated - 180);
				//rotatingHourGlass = false;
			}
			middlePlugTR.Rotate (rot, 0, 0);
		}
				
		//if (rotateFinished) {
		//}
		if (waiting3) {
			waitTime3 = waitTime3 + Time.deltaTime;
			if (waitTime3 > 2.0f) {
				waiting3 = false;
				Debug.Log ("done Waiting3..");


				// find stuck
				stuck = 0;
				fallen = 0;

				Vector3[] stuckGrain = new Vector3[batchCount * maxBatch];
				Vector3 tempGrain;
				for (int b = 1; b <= batches; b++) {
					for (int d = 1; d <= batchCount; d++) {
						string name = "sandGrain_B" + b + "_" + d;
						GameObject dup = GameObject.Find (name);
						if (dup.transform.position.y > 18.0f) {
							Debug.Log ("STUCK: " + name);
							stuckGrain [stuck] = dup.transform.position;
							stuck = stuck + 1;
						}
						if (dup.transform.position.y < 0.0f) {
							Debug.Log ("FALLEN: " + name);
							//stuckGrain [stuck] = dup.transform.position;
							fallen = fallen + 1;
						}





					}
				}

				// sort on y
				bool swapped;
				do {
					swapped = false;
					for (int s = 0; s < stuck - 1; s++) {
						if (stuckGrain [s].y > stuckGrain [s + 1].y) { // swap 
							tempGrain = stuckGrain [s];
							stuckGrain [s] = stuckGrain [s + 1];
							stuckGrain [s + 1] = tempGrain;
							swapped = true;
						}
					}
				} while (swapped == true);
				Debug.Log ("done Sort..");


				for (int s = 0; s < stuck; s++) {
					float dist = Mathf.Sqrt(stuckGrain [s].x * stuckGrain [s].x + stuckGrain [s].z * stuckGrain [s].z);
					Debug.Log ("S:  " + s + "  pnt: " + stuckGrain [s].x + "," + stuckGrain [s].y + "," + stuckGrain [s].z + "  Dist: " + dist);
				}

				dropping2 = true;
				middleOpen = false;
				//doRotation = true;
				//waiting = false;
				//Debug.Log ("done Waiting..");





			}
		}




		if (dropping2) {
			if (!middleOpen) {
				allDropped = false;
				middlePlugMR.enabled = false;
				middlePlugMC.enabled = false;
				middleOpen = true;
			}

			int dropped = 0;
			for (int b = 1; b <= batches; b++) {
				for (int d = 0; d < batchCount; d++) {
					int idx = (b - 1) * batchCount + d;
					if (duplicates [idx].transform.position.y < 13.0f)
						dropped = dropped + 1;
				}
			}
			Debug.Log ("dropped = " + dropped);
			if (dropped == batches * batchCount - stuck - fallen) {
				dropping2 = false;
				//waiting2 = true;
				//waitTime2 = 0.0f;
				//allDropped = true;
				//Debug.Log ("set Waiting2..");
			}
		}
		*/

/*
if (waiting6) {
	waitTime6 = waitTime6 + Time.deltaTime;
	if (waitTime6 > 1.0f) {
		waiting6 = false;

		//writeStartPoints ("endPointsb12.txt");
		//writeStartPoints ("endPointsb20.txt");
		//Debug.Log ("waiting6 =" + waiting6);
		//writeStartPoints ("endPointsb40.txt");

		rotatingHourGlass = true;


		rotated = 0.0f; 
		rotateSpeed = 0.0f; // deg / sec
		Debug.Log ("done Waiting6..");

		bottomHalfHour3MC.enabled = true;
		middlePlugMR.enabled = true;
		middlePlugMC.enabled = true;

	}
}


if (waiting7) {
	waitTime7 = waitTime7 + Time.deltaTime;
	doPushBack ("DoneRot");
	if (waitTime7 > 2.0f) {
		waiting7 = false;


		//writeStartPoints ("turnedPointsb8.txt");

		rattling = true;


	}
}

if (rattling) {
	rattleTime = rattleTime + Time.deltaTime;
	shakeAllGrains (0.1f);
	doPushBack ("Rattle");
	if (rattleTime > 0.2f) {
		rattling = false;
		waiting8 = true;
	}
}




if (waiting8) {
	waitTime8 = waitTime8 + Time.deltaTime;
	//doPushBack ("postRattle");
	if (waitTime8 > 2.0f) {
		waiting8 = false;
		writeStartPoints ("rattledPointsb8.txt");
	}
}
*/
