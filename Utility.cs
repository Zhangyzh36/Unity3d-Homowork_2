﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Utility
{

    public class Director : System.Object
    {
        private static Director _instance;

        public SceneController currentSceneController { get; set; }

        public static Director getInstance()
        {

            if (_instance == null)
            {
                _instance = new Director();
            }

            return _instance;
        }
    }

    public interface SceneController
    {
        void loadResources();
    }

    public interface UserAction
    {
        void moveBoatToTheOtherCoast();
        void characterBeClicked(zyzCharacterController _characterController);
        void restartGame();
    }

    public class Move : MonoBehaviour
    {
        int status;
        Vector3 dest;
        Vector3 intermediate;

        float speed = 15;

        public void setDestination(Vector3 newDest)
        {
            dest = newDest;
            if (dest.y == transform.position.y)
            {
                intermediate = newDest;
                status = 2;
            }
            else if (dest.y < transform.position.y)
            {
                intermediate.x = dest.x;
                intermediate.y = transform.position.y;
            }
            else if (dest.y > transform.position.y)
            {
                intermediate.y = dest.y;
                intermediate.x = transform.position.x;
            }
            status = 1;
        }


        void Update()
        {
            if (status == 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, intermediate, speed * Time.deltaTime);
                if (transform.position == intermediate)
                {
                    status = 2;
                }
            }
            else if (status == 2)
            {
                transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
                if (transform.position == dest)
                {
                    status = 0;
                }
            }

        }

        
        public void clear()
        {
            status = 0;
        }
    }

    public class zyzCharacterController
    {
        GameObject character;
        ClickGUI clickGUI;

        Move move;
        CoastController coastController;

        bool type; // false->priest, ture->devil
        bool onBoat;

        public zyzCharacterController(string characterString)
        {

            if (characterString == "priest")
            {
                character = Object.Instantiate(Resources.Load("Prefabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
                type = false;
            }
            else if (characterString == "devil")
            {
                character = Object.Instantiate(Resources.Load("Prefabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
                type = true;
            }

            move = character.AddComponent(typeof(Move)) as Move;
            clickGUI = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
            clickGUI.setController(this);
        }


        public void setName(string newName)
        {
            character.name = newName;
        }

        public string getName()
        {
            return character.name;
        }

        public void setPosition(Vector3 newPosition)
        {
            character.transform.position = newPosition;
        }

        public Vector3 getPosition()
        {
            return character.transform.position;
        }

        public bool getType()
        {
            return type;
        }

        public CoastController getCoastController()
        {
            return coastController;
        }


        public bool isOnBoat()
        {
            return onBoat;
        }

        public void moveToSomeplace(Vector3 destination)
        {
            move.setDestination(destination);
        }


        public void toBoat(BoatController boatController)
        {
            //leave the coast and let the boat be the parent of the character
            coastController = null;
            character.transform.parent = boatController.getGameobject().transform;
            onBoat = true;
        }

        public void toCoast(CoastController _coastController)
        {
            coastController = _coastController;
            character.transform.parent = null;
            onBoat = false;
        }


        public void clear()
        {
            move.clear();
            coastController = (Director.getInstance().currentSceneController as FirstController).leftCoast;
            //get to the left coast
            toCoast(coastController);
            //get to the empty position in left coast
            setPosition(coastController.getEmptyPosition());
            //this character get to the coast(the coast is 'got' by the character)
            coastController.toCoast(this);
        }
    }

    public class CoastController
    {
        GameObject coast;
        zyzCharacterController[] passengers;

        Vector3 leftPosition = new Vector3(-7f, 0.5f, 0);
        Vector3 rightPosition = new Vector3(7f, 0.5f, 0);
        Vector3[] positions;
        bool leftRight;

        public CoastController(string _leftRight)
        {
            positions = new Vector3[] {new Vector3(-5.3F,1.7F,0), new Vector3(-6F,1.7F,0), new Vector3(-6.7F,1.7F,0),
                new Vector3(-7.4F,1.7F,0), new Vector3(-8.1F,1.7F,0), new Vector3(-8.8F,1.7F,0)};

            passengers = new zyzCharacterController[6];

            if (_leftRight == "left")
            {
                coast = Object.Instantiate(Resources.Load("Prefabs/Coast", typeof(GameObject)), leftPosition, Quaternion.identity, null) as GameObject;
                coast.name = "left";
                leftRight = true;
            }
            else
            {
                coast = Object.Instantiate(Resources.Load("Prefabs/Coast", typeof(GameObject)), rightPosition, Quaternion.identity, null) as GameObject;
                coast.name = "right";
                leftRight = false;
            }
        }

        public int getEmptyIndex()
        {
            for (int i = 0; i < passengers.Length; i++)
            {
                if (passengers[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector3 getEmptyPosition()
        {
            Debug.Log(getEmptyIndex());
            Vector3 pos = positions[getEmptyIndex()];
            pos.x *= (leftRight ? 1 : -1);
            return pos;
        }

        public void toCoast(zyzCharacterController characterController)
        {
            passengers[getEmptyIndex()] = characterController;
        }

        public zyzCharacterController offCoast(string passenger_name)
        {   // 
            for (int i = 0; i < passengers.Length; i++)
            {
                if (passengers[i] != null && passengers[i].getName() == passenger_name)
                {
                    zyzCharacterController charactorController = passengers[i];
                    Debug.Log(i + " off Coast");
                    passengers[i] = null;
                    Debug.Log(passengers[i] == null);
                    return charactorController;
                }
            }

            return null;
        }

        public bool getLeftRight()
        {
            return leftRight;
        }

        public int[] getCharacterNumber()
        {
            int[] num = { 0, 0 };
            for (int i = 0; i < passengers.Length; i++)
            {
                if (passengers[i] == null)
                    continue;
                if (passengers[i].getType() == false)
                {   
                    num[0]++;
                }
                else
                {
                    num[1]++;
                }
            }
            return num;
        }

        public void clear()
        {
            for (int i = 0; i < passengers.Length; i++)
            {
                passengers[i] = null;
            }
        }

        public class BoatController
        {
            GameObject boat;
            Move move;
            Vector3 leftPosition = new Vector3(-3f, 0.7f, 0);
            Vector3 rightPosition = new Vector3(3f, 0.7f, 0);
            Vector3[] leftPositionitions;
            Vector3[] rightPositionitions;

            bool leftRight;  
            zyzCharacterController[] passenger = new zyzCharacterController[2];

            public BoatController()
            {
                leftRight = true;

                leftPositionitions = new Vector3[] { new Vector3(-4F, 1.2F, 0), new Vector3(-2F, 1.2F, 0) };
                rightPositionitions = new Vector3[] { new Vector3(2F, 1.2F, 0), new Vector3(4F, 1.2F, 0) };

                boat = Object.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject)), leftPosition, Quaternion.identity, null) as GameObject;
                boat.name = "boat";

                move = boat.AddComponent(typeof(Move)) as Move;
                boat.AddComponent(typeof(ClickGUI));
            }


            public void Move()
            {
                if (leftRight == false)
                {
                    move.setDestination(leftPosition);
                    leftRight = true;
                }
                else
                {
                    move.setDestination(rightPosition);
                    leftRight = false;
                }
            }

            public int getEmptyIndex()
            {
                for (int i = 0; i < passenger.Length; i++)
                {
                    if (passenger[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public bool isEmpty()
            {
                for (int i = 0; i < passenger.Length; i++)
                {
                    if (passenger[i] != null)
                    {
                        return false;
                    }
                }
                return true;
            }

            public Vector3 getEmptyPosition()
            {
                Vector3 pos;
                int emptyIndex = getEmptyIndex();
                if (leftRight == false)
                {
                    pos = rightPositionitions[emptyIndex];
                }
                else
                {
                    pos = leftPositionitions[emptyIndex];
                }
                return pos;
            }

            public void toBoat(zyzCharacterController _characterController)
            {
                int index = getEmptyIndex();
                passenger[index] = _characterController;
            }

            public zyzCharacterController offBoat(string passenger_name)
            {
                for (int i = 0; i < passenger.Length; i++)
                {
                    if (passenger[i] != null && passenger[i].getName() == passenger_name)
                    {
                        zyzCharacterController charactorController = passenger[i];
                        passenger[i] = null;
                        return charactorController;
                    }
                }
                return null;
            }

            public GameObject getGameobject()
            {
                return boat;
            }

            public bool getLeftRight()
            {
                return leftRight;
            }

            public int[] getCharacterNumber()
            {
                int[] num = { 0, 0 };
                for (int i = 0; i < passenger.Length; i++)
                {
                    if (passenger[i] == null)
                        continue;
                    if (passenger[i].getType() == false)
                    {   
                        num[0]++;
                    }
                    else
                    {
                        num[1]++;
                    }
                }
                return num;
            }

            public void clear()
            {
                move.clear();
                if (leftRight == false)
                {
                    Move();
                }
                passenger = new zyzCharacterController[2];
            }
        }

    }

    public class BoatController
    {
        GameObject boat;
        Move move;
        Vector3 leftPosition = new Vector3(-3f, 0.7f, 0);
        Vector3 rightPosition = new Vector3(3f, 0.7f, 0);
        Vector3[] leftPosiitons;
        Vector3[] rightPositions;

        bool leftRight; 

        zyzCharacterController[] passenger = new zyzCharacterController[2];

        public BoatController()
        {
            leftRight = true;

            leftPosiitons = new Vector3[] { new Vector3(-4F, 1.2F, 0), new Vector3(-2F, 1.2F, 0) };
            rightPositions = new Vector3[] { new Vector3(2F, 1.2F, 0), new Vector3(4F, 1.2F, 0) };

            boat = Object.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject)), leftPosition, Quaternion.identity, null) as GameObject;
            boat.name = "boat";

            move = boat.AddComponent(typeof(Move)) as Move;
            boat.AddComponent(typeof(ClickGUI));
        }


        public void Move()
        {
            if (leftRight == false)
            {
                move.setDestination(leftPosition);
                leftRight = true;
            }
            else
            {
                move.setDestination(rightPosition);
                leftRight = false;
            }
        }

        public bool isEmpty()
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if (passenger[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        public int getEmptyIndex()
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if (passenger[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector3 getEmptyPosition()
        {
            Vector3 pos;
            int emptyIndex = getEmptyIndex();
            if (leftRight == false)
            {
                pos = rightPositions[emptyIndex];
            }
            else
            {
                pos = leftPosiitons[emptyIndex];
            }
            return pos;
        }

        public void toBoat(zyzCharacterController characterController)
        {
            int index = getEmptyIndex();
            passenger[index] = characterController;
        }

        public zyzCharacterController offBoat(string passenger_name)
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if (passenger[i] != null && passenger[i].getName() == passenger_name)
                {
                    zyzCharacterController charactorController = passenger[i];
                    passenger[i] = null;
                    return charactorController;
                }
            }
            
            return null;
        }

        public GameObject getGameobject()
        {
            return boat;
        }

        public bool getLeftRight()
        { 
            return leftRight;
        }

        public int[] getCharacterNumber()
        {
            int[] num = { 0, 0 };
            for (int i = 0; i < passenger.Length; i++)
            {
                if (passenger[i] == null)
                    continue;

                if (passenger[i].getType() == false)
                {   
                    num[0]++;
                }
                else
                {
                    num[1]++;
                }

            }
            return num;
        }

        public void clear()
        {
            move.clear();
            if (leftRight == false)
            {
                Move();
            }
            passenger[0] = passenger[1] = null;
        }
    }
}