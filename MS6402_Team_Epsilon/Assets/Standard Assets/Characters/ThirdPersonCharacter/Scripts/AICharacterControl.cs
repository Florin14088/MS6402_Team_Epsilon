using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    #region REQUIRED COMPONENTS
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    #endregion
    public class AICharacterControl : MonoBehaviour
    {
        #region Own CLASSES.................................................................................................
        [Serializable] public class General_Parameters
        {
            public Transform target;//this is the target, NPC will behave according to "Contact_Actions"
            public Transform possibleTarget;//whenever function "GetTargetInRadius" get's an object with any tag from "interestTags", the NPC will do a raycast to that object to see if it can SEE it or not.
            public Transform drawTarget;//a sound (something hit the floor, music, etc) will alert all the NPCs within a range. An NPC with target == null will come to this target
            public float sightRadius = 30;//how far the AI can see
        }

        [Serializable] public class Horror_AI
        {
            public List<string> interestTags = new List<string>() { "Player" };//objects with any tags from this list will make this NPC curious
            [HideInInspector] public bool b_firstTime = true;//if true, it means that this NPC is seeing the player for the first time in this gaming session
            [HideInInspector] public enum Contact_Actions { Nothing, Warp, Stalk, Alert, Aggressive, Kill };//what will the NPC do when ignoreSights = 0 and the NPC spots the player (NOTE: Nothing = AFK NPC)
            public Contact_Actions contactActions;
            [HideInInspector] public enum START_FROM_Contact_Actions { Nothing, Warp, Stalk, Alert, Aggressive, Kill };//NPC will start from the action picked from this enum (the above enum is overall enum)
            public Contact_Actions startFrom_contactActions;//suggest to start from Warp unless you want the NPC to be AFK (passive to the environment)
            [HideInInspector] public RaycastHit hit;//used in NPC_Raycast_Possible_Target
        }

        [Serializable] public class ActionBehaviour_Warp
        {            
            public int charges = 2;//how many times the NPC will use the current Behaviour before moving to the next Behaviour
            public float min_distanceWarp = 6;
            public float max_distanceWarp = 10;
            public float pickedDistance;//random between min and max. A new distance is picked every time when player moves
            public float radiusWarp = 30;//pick a point as destination in a sphere with radius "radiusWarp". The point will always be on NavMesh
        }

        [Serializable] public class ActionBehaviour_Stalk
        {
            public int charges = 2;//how many times the NPC will use the current Behaviour before moving to the next Behaviour
            public float min_distanceStalk = 6;
            public float max_distanceStalk = 10;
            public float pickedDistance;//random between min and max. A new distance is picked every time when player moves
            public Vector3 playerPosition_new;//current position of the player
            public Vector3 playerPosition_old = Vector3.zero;//the previous position of the player. If this one is not the same with "playerPosition_new", a new "pickedDistance" is picked
        }

        [Serializable] public class ActionBehaviour_Alert
        {
            public int charges = 2;//how many times the NPC will use the current Behaviour before moving to the next Behaviour
            public float min_distanceAlert = 6;
            public float max_distanceAlert = 10;
            public float pickedDistance;//random between min and max. A new distance is picked every "cooldown" seconds. All NPCs (if any) in this radius will have "drawTarget = target of this NPC"
        }

        [Serializable] public class ActionBehaviour_Aggressive
        {
            public int charges = 2;//how many times the NPC will use the current Behaviour before moving to the next Behaviour
            public float min_distanceStalk = 6;
            public float max_distanceStalk = 10;
            public float pickedDistance;//random between min and max. A new distance is picked every time when player moves
            [HideInInspector] public float nextCooldown = 0;
            public Vector3 playerPosition_new;//current position of the player
            public Vector3 playerPosition_old = Vector3.zero;//the previous position of the player. If this one is not the same with "playerPosition_new", a new "pickedDistance" is picked
            public float cooldownAttack = 3f;
            [HideInInspector] public float nextCooldownAttack = 0;
            public GameObject prefabPickUp_FEAR;//instantiate at player's position to give fear
        }

        [Serializable] public class ActionBehaviour_Kill
        {
            public float min_distanceStalk = 6;
            public float max_distanceStalk = 10;
            public float pickedDistance;//random between min and max. A new distance is picked every time when player moves
            [HideInInspector] public float nextCooldown = 0;
            public Vector3 playerPosition_new;//current position of the player
            public Vector3 playerPosition_old = Vector3.zero;//the previous position of the player. If this one is not the same with "playerPosition_new", a new "pickedDistance" is picked
            public float cooldownAttack = 3f;
            [HideInInspector] public float nextCooldownAttack = 0;
            public GameObject prefabPickUp_FEAR;//instantiate at player's position to give fear
        }

        [Serializable] public class Patrolling
        {
            public bool b_canPatrol = false;// if this is false, NPC will be static if player is further away than chase distance
            public bool b_RecyclePoints = true;// if this is false, NPC will stop patrolling when reaching last object in the array
            public GameObject[] patrolPoints;
            public float stoppingDistance = 3;// how close to the current patrol point must be in order to switch to next one or pause. Should be AT LEAST EQUAL with NavMeshAgent.StoppingDistance
            public bool b_pause_at_destination = false;//true = evert time when the NPC reaches the current waypoint, the NPC will pause for pauseTimer
            public float pauseTimer = 5;
            [HideInInspector] public float containerPauseTimer = 0;
            public float cooldown = 1;
            [HideInInspector] public float nextCooldown = 0;
            [HideInInspector] public int waypoint_index = 0;
            [HideInInspector] public float waypoint_dist; //distance between current waypoint and this object = 0;
        }

        [Serializable] public class RandomWandering
        {
            public bool b_enabled = false;
            public float wanderRadius = 10;
            public float wanderTimer = 15;
            public float stoppingDistance = 3;// how close to the current DESTINATION point must be in order to stop
            [HideInInspector] public float timer;
            [HideInInspector] public Vector3 newPos;
        }


        #endregion..........................................................................................................



        #region PUBLIC......................................................................................................
        [Header("General Matter")]
        public General_Parameters cls_GenPar = new General_Parameters();

        [Header("Core A.I. Parameters")]
        public Horror_AI cls_CoreHorror = new Horror_AI();

        [Header("Warp Action Parameters")]
        public ActionBehaviour_Warp cls_Warp = new ActionBehaviour_Warp();

        [Header("Stalk Action Parameters")]
        public ActionBehaviour_Stalk cls_Stalk = new ActionBehaviour_Stalk();

        [Header("Alert Action Parameters")]
        public ActionBehaviour_Alert cls_Alert = new ActionBehaviour_Alert();

        [Header("Aggressive Action Parameters")]
        public ActionBehaviour_Aggressive cls_Aggressive = new ActionBehaviour_Aggressive();

        [Header("Kill Action Parameters")]
        public ActionBehaviour_Kill cls_Kill = new ActionBehaviour_Kill();

        [Header("Patrolling Behavious")]
        public Patrolling cls_Patrol = new Patrolling();

        [Header("Random Wander Behaviour")]
        public RandomWandering cls_RandWand = new RandomWandering();
        #endregion..........................................................................................................



        #region PRIVATE.....................................................................................................
        public NavMeshAgent agent { get; private set; }
        public ThirdPersonCharacter character { get; private set; }

        private List<Light> mannedLights = new List<Light>();
        private bool b_lightManipulated = false;
        #endregion..........................................................................................................





        #region PRE DEFINITED FUNCTIONS.....................................................................................

        private void Start()
        {
            agent = GetComponentInChildren<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;


            #region SAVE SOME TIME BY PREVENTING BUGS

            if(cls_Patrol.stoppingDistance < agent.stoppingDistance) cls_Patrol.stoppingDistance = agent.stoppingDistance;//WHY? because if not, the NPC will never go to the next waypoint
            if(cls_RandWand.stoppingDistance < agent.stoppingDistance) cls_RandWand.stoppingDistance = agent.stoppingDistance;//WHY? because if not, the NPC will never go to the next waypoint

            #endregion

            cls_CoreHorror.contactActions = cls_CoreHorror.startFrom_contactActions;//NPC will start from phase indicated by "startFrom_contactActions"
            cls_Warp.pickedDistance = UnityEngine.Random.Range(cls_Warp.min_distanceWarp, cls_Warp.max_distanceWarp);//get the first picked distance for Warp Behaviour
            cls_Stalk.pickedDistance = UnityEngine.Random.Range(cls_Stalk.min_distanceStalk, cls_Stalk.max_distanceStalk);//get the first picked distance for Stalk Behaviour
            cls_Alert.pickedDistance = UnityEngine.Random.Range(cls_Alert.min_distanceAlert, cls_Alert.max_distanceAlert);//get the first picked distance for Alert Behaviour


            cls_CoreHorror.contactActions = cls_CoreHorror.startFrom_contactActions;


        }//Start


        private void Update()
        {
            AI_Brain();

            AlterLights_ON();

        }//Update
        #endregion..........................................................................................................



        #region OWN FUNCTIONS...............................................................................................
        void AI_Brain()
        {
            GetTargetInRadius(transform.position, cls_GenPar.sightRadius);//attempt to get a possibleTarget if NPC has no target

            #region NO TARGET OR DRAW TARGET
            if (cls_GenPar.target == null)//no target and no location to investigate
            {

                if(cls_GenPar.drawTarget == null)
                {
                    if (cls_Patrol.b_canPatrol && cls_RandWand.b_enabled)
                    {
                        agent.path = null;
                        cls_Patrol.b_canPatrol = false;
                        cls_RandWand.b_enabled = false;
                        return;
                    }
                    Patrolling_Behaviour(); //function is called, but will run if allowed by bool
                    Random_Wander_Behaviour(); //function is called, but will run if allowed by bool
                }
                
            }
            #endregion

            #region HAVE TARGET OR DRAW TARGET


            #region DRAW TARGET INVESTIGATE
            if (cls_GenPar.drawTarget && cls_GenPar.target == null)//IF NPC GOT A NOISE TO INVESTIGATE AND NO MAIN TARGET IS AVAILABLE, GO TO DRAW TARGET
            {
                if (cls_CoreHorror.contactActions != Horror_AI.Contact_Actions.Warp
                    && cls_CoreHorror.contactActions != Horror_AI.Contact_Actions.Nothing) agent.SetDestination(cls_GenPar.drawTarget.position);
            }
            #endregion

            #region POSSIBLE TARGET RAYCAST
            if (cls_GenPar.possibleTarget)//NO TARGET BUT A POSSIBLE TARGET DETECTED. DO A RAYCAST WHILE DOING THE OTHER NON-TARGET BEHAVIOURS TO SEE IF NPC CAN GET THE TARGET
            {
                NPC_Raycast_Possible_Target();
            }
            #endregion

            #region MAIN TARGET AVAILABLE, ACT ACCORDING TO ACTIONS
            if (cls_GenPar.target)//NPC GOT A CLEAR TARGET TO GO
            {
                if (cls_GenPar.drawTarget) cls_GenPar.drawTarget = null;//CLEAR ANY DRAW TARGET BECAUSE NO NEED TO INVESTIGATE NOISES WHEN A TARGET IS AVAILABLE
                if(cls_CoreHorror.contactActions != Horror_AI.Contact_Actions.Warp
                    && cls_CoreHorror.contactActions != Horror_AI.Contact_Actions.Nothing) agent.SetDestination(cls_GenPar.target.position);
            }
            #endregion


            if (agent.remainingDistance > agent.stoppingDistance && cls_CoreHorror.contactActions != Horror_AI.Contact_Actions.Warp
                    && cls_CoreHorror.contactActions != Horror_AI.Contact_Actions.Nothing)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else//CLOSE ENOUGH TO STOP GOING TO TARGET (DO A "Contact_Actions" ACTION MAYBE)
            {
                character.Move(Vector3.zero, false, false);

                if(cls_CoreHorror.contactActions == Horror_AI.Contact_Actions.Aggressive)
                {
                    if(Time.time > cls_Aggressive.nextCooldown)
                    {
                        cls_Aggressive.nextCooldown = Time.time + cls_Aggressive.cooldownAttack;
                        StartCoroutine(SendDamageFEAR(cls_Aggressive.prefabPickUp_FEAR));
                    }
                }

                if(cls_CoreHorror.contactActions == Horror_AI.Contact_Actions.Kill)
                {
                    if(Time.time > cls_Kill.nextCooldown)
                    {
                        cls_Kill.nextCooldown = Time.time + cls_Kill.cooldownAttack;
                        StartCoroutine(SendDamageFEAR(cls_Kill.prefabPickUp_FEAR));
                    }
                }

                if (cls_GenPar.target)
                {
                    switch (cls_CoreHorror.contactActions)
                    {
                        case Horror_AI.Contact_Actions.Nothing:
                            ContactAction_Nothing();
                            break;

                        case Horror_AI.Contact_Actions.Warp:
                            ContactAction_Warp();
                            break;

                        case Horror_AI.Contact_Actions.Stalk:
                            ContactAction_Stalk();
                            break;

                        case Horror_AI.Contact_Actions.Alert:
                            ContactAction_Alert();
                            break;

                        case Horror_AI.Contact_Actions.Aggressive:
                            ContactAction_Aggressive();
                            break;

                        case Horror_AI.Contact_Actions.Kill:
                            ContactAction_Kill();
                            break;

                        default:
                            break;
                    }
                }


            }


            #endregion

        }//AI_Brain

        

        #region USED BY HORROR AI CORE WHEN HAVING (OR SEARCHING) TARGET

        public void GetTargetInRadius(Vector3 center, float radius)//Get possible target that will be used to raycast at it in order to decide if we can see it or not
        {
            #region Put your cursor here to find WHAT IS THIS FUNCTION
            //BASIC Physics.OverlapSphere
            //Returns an array with all colliders touching or inside the sphere. 
            //Learn this at https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
            //Here this function is called when the NPC have no target
            #endregion

            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            int i = 0;
            while (i < hitColliders.Length)
            {
                //print(hitColliders[i].gameObject.name);

                foreach (string s in cls_CoreHorror.interestTags)
                {
                    if (hitColliders[i].gameObject.tag == s)
                    {
                        //cls_GenPar.target = hitColliders[i].gameObject;
                        //SetTarget(hitColliders[i].gameObject.transform);
                        Set_POSSIBLE_Target(hitColliders[i].gameObject.transform);
                        return;
                    }
                    else i++;
                }


            }
        }


        void NPC_Raycast_Possible_Target()
        {
            #region Get distance and angle needed for the behaviour
            float dist = Vector3.Distance(cls_GenPar.possibleTarget.transform.position, gameObject.transform.position);
            Vector3 targetDir = cls_GenPar.possibleTarget.transform.position - gameObject.transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);//find the angle between agent and target
            #endregion

            if (Physics.Raycast(gameObject.transform.position, targetDir, out cls_CoreHorror.hit, dist))
            {
                if (cls_CoreHorror.hit.collider != null)
                {
                    foreach(string s in cls_CoreHorror.interestTags)
                    {
                        if (cls_CoreHorror.hit.collider.gameObject.transform.root.tag == s)
                        {
                            SetTarget(cls_CoreHorror.hit.collider.gameObject.transform);//NOW THAT NPC CAN SEE THE POSSIBLE TARGET, SET TARGET                            
                        }
                        else
                        {
                            cls_GenPar.drawTarget = cls_GenPar.target;
                            cls_GenPar.target = null;//cannot see the target, target is null. This will trigger "GetTargetInRadius" and a possibleTarget will be obtained
                        }

                    }

                }

            }

        }//NPC_Raycast_Possible_Target

        #endregion



        void Patrolling_Behaviour()
        {
            #region Put your cursor here to find WHAT IS THIS FUNCTION
            //THIS FUNCTION ALLOWS THE NPC TO PATROLL A PRE DEFINED LOCATIONS
            //WHEN ALL LOCATIONS HAVE BEEN PATROLLED, THE NPC CAN EITHER START AGAIN OR BE IDLE
            #endregion


            if (cls_Patrol.b_canPatrol == false) return;// if can't patrol, skip

            #region CALCULATING THE DISTANCE BETWEEN NPC AND DESTINATION
            Vector3 destination = cls_Patrol.patrolPoints[cls_Patrol.waypoint_index].transform.position;
            Vector3 myPos = gameObject.transform.position;
            cls_Patrol.waypoint_dist = Vector3.Distance(destination, myPos);// calculate the distance between current waypoint and this object
            #endregion

            if (cls_Patrol.waypoint_dist > cls_Patrol.stoppingDistance)
            {
                agent.SetDestination(cls_Patrol.patrolPoints[cls_Patrol.waypoint_index].transform.position);// destination: waypoint   if not close enough to waypoint
            }
            else if (cls_Patrol.waypoint_dist <= cls_Patrol.stoppingDistance)// if close enough to waypoint
            {

                #region IF THE NPC NEEDS TO PAUSE AT THE DESTINATION
                if (cls_Patrol.containerPauseTimer > 0 && cls_Patrol.b_pause_at_destination)
                {
                    if (Time.time > cls_Patrol.nextCooldown)
                    {
                        cls_Patrol.nextCooldown = Time.time + cls_Patrol.cooldown;
                        cls_Patrol.containerPauseTimer -= cls_Patrol.cooldown;
                    }
                    return;
                }

                if (cls_Patrol.containerPauseTimer < cls_Patrol.pauseTimer && cls_Patrol.b_pause_at_destination) cls_Patrol.containerPauseTimer = cls_Patrol.pauseTimer;//resets the value of the waiting time container
                #endregion

                #region WHAT HAPPENS AFTER THE NPC REACHES THE CURRENT DESTINATION WAYPOINT

                #region THERE IS ANOTHER WAYPOINT TO GO
                if (cls_Patrol.waypoint_index < cls_Patrol.patrolPoints.Length) cls_Patrol.waypoint_index++;// go to next waypoint if current waypoint is not the last one
                #endregion

                #region THIS WAS THE LAST WAYPOINT BUT...
                if (cls_Patrol.waypoint_index >= cls_Patrol.patrolPoints.Length && cls_Patrol.b_RecyclePoints)// if no more waypoints to go and b_RecyclePoints is true
                {
                    cls_Patrol.waypoint_index = 0;// go to the first waypoint
                }
                else if (cls_Patrol.waypoint_index >= cls_Patrol.patrolPoints.Length && cls_Patrol.b_RecyclePoints == false)// if no more waypoints to go and b_RecyclePoints is false
                {
                    cls_Patrol.waypoint_index = 0;
                    cls_Patrol.b_canPatrol = false;// cannot patrol anymore
                }
                #endregion

                #endregion
            }

        }//Patrolling_Behaviour



        #region USED BY RANDOM WANDER BEHAVIOUR
        void Random_Wander_Behaviour()
        {
            #region Put your cursor here to find WHAT IS THIS FUNCTION
            //THIS FUNCTION ALLOWS THE NPC TO WANDER RANDOMLY OVER THE NAVMESH.
            //NPC WILL GO TO A RANDOMLY PICKED DESTINATION, THEN WILL WAIT (OR NOT), OVER AND OVER AGAIN
            #endregion


            if (cls_RandWand.b_enabled == false) return;

            cls_RandWand.timer += Time.deltaTime;

            if (cls_RandWand.timer >= cls_RandWand.wanderTimer)
            {
                cls_RandWand.newPos = RandomNavSphere(transform.position, cls_RandWand.wanderRadius, -1);
                agent.SetDestination(cls_RandWand.newPos);
                cls_RandWand.timer = 0;
            }

            if (Vector3.Distance(cls_RandWand.newPos, gameObject.transform.position) <= cls_RandWand.stoppingDistance)
            {
                SetTarget(null);
            }


        }//Random_Wander_Behaviour


        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            #region Put your cursor here to find WHAT IS THIS FUNCTION
            //THIS FUNCTION RETURNS A RANDOM POINT ON THE NAVMESH WITHIN A RADIUS.
            //YOU GIVE VALUES FOR cls_PatrolS: WHERE IS CENTRE OF SEARCHING AREA, HOW FAR TO GO AND WHAT LAYER/LAYERS TO USE
            #endregion

            Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
            randDirection += origin;

            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;

        }//RandomNavSphere
        #endregion



        #region CONTACT ACTIONS

        void ContactAction_Nothing()
        {
            if (cls_GenPar.target != null) cls_GenPar.target = null;
            if (cls_GenPar.possibleTarget != null) cls_GenPar.possibleTarget = null;
            if (cls_GenPar.drawTarget != null) cls_GenPar.drawTarget = null;
            //if (cls_GenPar.sightRadius > 0) cls_GenPar.sightRadius = 0;
            
        }//ContactAction_Nothing



        #region WARP
        void ContactAction_Warp()
        {

            if(Vector3.Distance(cls_GenPar.target.transform.position, transform.position) <= cls_Warp.pickedDistance)
            {
                cls_Warp.pickedDistance = UnityEngine.Random.Range(cls_Warp.min_distanceWarp, cls_Warp.max_distanceWarp);
                
                if(cls_Warp.charges > 0) cls_Warp.charges--;

                Warping_NPC();

                if (cls_Warp.charges <= 0)
                {
                    ContactAction_Nothing();
                    cls_CoreHorror.contactActions = Horror_AI.Contact_Actions.Stalk;
                }

            }       


        }//ContactAction_Warp


        void Warping_NPC()
        {
            StartCoroutine(LightsFlicker());
            agent.Warp(RandomNavSphere(transform.position, cls_Warp.radiusWarp, -1));//teleport the agent
            cls_Warp.pickedDistance = UnityEngine.Random.Range(cls_Warp.min_distanceWarp, cls_Warp.max_distanceWarp);

        }//Warping_NPC


        void AlterLights_OFF(Vector3 center, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            foreach (Collider hitC in hitColliders)
            {
                if (hitC.gameObject.GetComponent<Light>())
                {
                    hitC.gameObject.GetComponent<Light>().enabled = false;
                    mannedLights.Add(hitC.gameObject.GetComponent<Light>());
                }

            }

        }//AlterLights_OFF

        void AlterLights_ON()
        {
            if (b_lightManipulated == false) return;

            if (mannedLights.Count == 0 && b_lightManipulated == true)
            {
                b_lightManipulated = false;
                return;
            } 

            foreach(Light l in mannedLights)
            {
                l.enabled = true;
                mannedLights.Remove(l);
            }

        }//AlterLights_ON


        IEnumerator LightsFlicker()
        {
            AlterLights_OFF(transform.position, 10);
            yield return new WaitForSeconds(0.4f);
            yield return new WaitForEndOfFrame();
            b_lightManipulated = true;

        }//LightsFlicker

        #endregion


        void ContactAction_Stalk()
        {
            if (Vector3.Distance(cls_GenPar.target.transform.position, transform.position) <= cls_Stalk.pickedDistance)
            {
                agent.SetDestination(transform.position);
                character.Move(Vector3.zero, false, false);
            }

            if (Vector3.Distance(cls_GenPar.target.transform.position, transform.position) <= cls_Stalk.pickedDistance/2)
            {
                Warping_NPC();
                ContactAction_Nothing();
            }

            cls_Stalk.playerPosition_new = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (cls_Stalk.playerPosition_old == Vector3.zero) cls_Stalk.playerPosition_old = cls_Stalk.playerPosition_new;

            if(cls_Stalk.playerPosition_old != cls_Stalk.playerPosition_new)
            {
                if(cls_Stalk.charges > 0) cls_Stalk.charges--;
                cls_Stalk.pickedDistance = UnityEngine.Random.Range(cls_Stalk.min_distanceStalk, cls_Stalk.max_distanceStalk);
                cls_Stalk.playerPosition_old = cls_Stalk.playerPosition_new;
            }

            if (cls_Stalk.charges <= 0)
            {
                ContactAction_Nothing();
                cls_CoreHorror.contactActions = Horror_AI.Contact_Actions.Alert;
            }

        }//ContactAction_Stalk
        


        void ContactAction_Alert()
        {
            if (Vector3.Distance(cls_GenPar.target.transform.position, transform.position) <= cls_Alert.pickedDistance)
            {
                cls_Alert.pickedDistance = UnityEngine.Random.Range(cls_Alert.min_distanceAlert, cls_Alert.max_distanceAlert);

                if (cls_Alert.charges > 0) cls_Alert.charges--;
                //PLAY SOME VOICES
                Warping_NPC();

                if (cls_Alert.charges <= 0)
                {
                    ContactAction_Nothing();
                    cls_CoreHorror.contactActions = Horror_AI.Contact_Actions.Aggressive;
                }

            }

        }//ContactAction_Alert


        #region Aggressive
        void ContactAction_Aggressive()
        {
            //part of this mechanic is in Function AI_Brain() in region MAIN TARGET

            cls_Aggressive.playerPosition_new = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (cls_Aggressive.playerPosition_old == Vector3.zero) cls_Aggressive.playerPosition_old = cls_Aggressive.playerPosition_new;

            if (cls_Aggressive.playerPosition_old != cls_Aggressive.playerPosition_new)
            {
                if (cls_Aggressive.charges > 0) cls_Aggressive.charges--;
                cls_Aggressive.pickedDistance = UnityEngine.Random.Range(cls_Aggressive.min_distanceStalk, cls_Aggressive.max_distanceStalk);
                cls_Aggressive.playerPosition_old = cls_Aggressive.playerPosition_new;
            }

            if (cls_Aggressive.charges <= 0)
            {
                ContactAction_Nothing();
                Warping_NPC();
                cls_CoreHorror.contactActions = Horror_AI.Contact_Actions.Kill;
            }

        }//ContactAction_Aggressive
        #endregion


        IEnumerator SendDamageFEAR(GameObject prefab)
        {
            //play creepy sounds
            yield return new WaitForSeconds(2);
            Instantiate(prefab, cls_GenPar.target.transform.position, cls_GenPar.target.transform.rotation);
            yield return new WaitForEndOfFrame();
            yield break;
        }//SendDamageFEAR


        void ContactAction_Kill()
        {
            //part of this mechanic is in Function AI_Brain() in region MAIN TARGET

            cls_Kill.playerPosition_new = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (cls_Kill.playerPosition_old == Vector3.zero) cls_Kill.playerPosition_old = cls_Kill.playerPosition_new;

            if (cls_Kill.playerPosition_old != cls_Kill.playerPosition_new)
            {
                cls_Kill.pickedDistance = UnityEngine.Random.Range(cls_Kill.min_distanceStalk, cls_Kill.max_distanceStalk);
                cls_Kill.playerPosition_old = cls_Kill.playerPosition_new;
            }

        }//ContactAction_Kill

        #endregion



        


        public void SetTarget(Transform target) { cls_GenPar.target = target; }//SetTarget
        public void Set_POSSIBLE_Target(Transform possibleTarget) { cls_GenPar.possibleTarget = possibleTarget; }//Set_POSSIBLE_Target
        #endregion..........................................................................................................


    }//END


}//Real END