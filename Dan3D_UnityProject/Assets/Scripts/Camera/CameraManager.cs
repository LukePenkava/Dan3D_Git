using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{    
    public Camera mainCamera;
    public Transform playerCameraBounds;
    FocusArea focusArea;

    public Vector2 focusAreaSize;
    public float zDistance = 10f;
    public float smoothing = 0.1f;
    public float verticalOffset = 1.5f; //Camera height is based on player's bounds Y pos and this offset
   
    bool cameraIsStatic = true;
    bool hasZone = false;
    float camPosDeltaX = 0;
    float smoothLookVelocityX;
    float smoothedFocusPositionX;    
    public Transform cameraZone;

    public enum CameraModes {
        Default,
        Trade,
        Crafting,
        StaticCentered,
        RefocusPlayer        
    }

    // void OnEnable() {
    //     AreaManager.AreaCreatedEvent += AreaCreated;
    // }

    // void OnDisable() {
    //     AreaManager.AreaCreatedEvent -= AreaCreated;
    // }
    

    public void Init() {
        focusArea = new FocusArea(playerCameraBounds, focusAreaSize);        
        SetCamera(CameraModes.Default);
    }

    public void SetZone(Transform zone) {        
        cameraZone = zone;
        hasZone = true;
        FocusPlayer();
    }

    void FocusPlayer() { focusArea.Update(playerCameraBounds, mainCamera, cameraZone);
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        mainCamera.transform.position = new Vector3(focusPosition.x, focusPosition.y, -zDistance);       
    }

    void LateUpdate() {
        if(cameraIsStatic) { return; }
        if(!hasZone) { return; }

        focusArea.Update(playerCameraBounds, mainCamera, cameraZone);
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        camPosDeltaX = focusPosition.x - mainCamera.transform.position.x;
        
        //print(focusPosition);
        mainCamera.transform.position = new Vector3(focusPosition.x, focusPosition.y, -zDistance);         
    }

    public void SetCamera(CameraModes mode, Vector3 playerPos = default(Vector3), Vector3 otherPos = default(Vector3)) {
    
        Vector3 centerPos = Vector3.zero;
        switch(mode) {
            case CameraModes.Default:
                //verticalOffset = verticalOffsetBase;
                cameraIsStatic = false;
                break;   
             case CameraModes.RefocusPlayer:
                // verticalOffset = verticalOffsetBase;
                // cameraIsStatic = false;                
                // focusArea.Recenter(target.characterBounds, focusAreaSize);   
                // FocusPlayer();            
                break;
            case CameraModes.Trade:
                // cameraIsStatic = true;
                // focusArea.Recenter(target.characterBounds, focusAreaSize);
                // centerPos = new Vector3( (playerPos.x + otherPos.x) / 2f, focusArea.center.y + verticalOffsetMenu, transform.position.z);                
                // transform.position = centerPos;                
                break;
            case CameraModes.Crafting:
                // cameraIsStatic = true;
                // focusArea.Recenter(target.characterBounds, focusAreaSize);
                // centerPos = new Vector3(playerPos.x, transform.position.y, transform.position.z);                
                // transform.position = centerPos;      
                break;
            case CameraModes.StaticCentered:
                // cameraIsStatic = true;
                // focusArea.Recenter(target.characterBounds, focusAreaSize);
                // centerPos = new Vector3(playerPos.x, focusArea.center.y + verticalOffsetBase, transform.position.z);                
                // transform.position = centerPos; 
                // //Use difference between centerPos
                // float dif = centerPos.x - (focusArea.center.x); //target.transform.position.x;                
                // focusArea.areaLeft += dif;
                // focusArea.areaRight += dif;             
                break;
            default:
                //verticalOffset = verticalOffsetBase;
                cameraIsStatic = false;           
                break;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(1,0,0, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea {
        public Vector2 center;
        public Vector2 velocity;
        public float areaLeft, areaRight, areaBottom, areaTop;

        public Vector3 testPos;
        

        //Focus Area is the red square around character, camera moves only when character goes outside/touches the focus area edge. If he moves inside, camera is not moving
        //Based on character collider(bounds) and size of focus area, find its center and positions
        public FocusArea(Transform targetBounds, Vector2 size) {

            areaLeft = targetBounds.position.x - size.x/2f;
            areaRight = targetBounds.position.x + size.x/2f;
            areaBottom = targetBounds.position.y - size.y/2f;
            areaTop = targetBounds.position.y + size.y/2f;

            velocity = Vector2.zero;
            center = new Vector2((areaLeft + areaRight) / 2, ( areaTop + areaBottom) / 2);            
       
            testPos = Vector3.zero;          
        }

        //Receive bounds from RaycastController
        //Compare bounds of the character ( character collider ) to focus area size. Ie is leftX of character bounds outside of focusArea leftX ? If so, move the camera
        public void Update(Transform targetBounds, Camera cam, Transform cameraZone) {
           
            if(cameraZone == null) { return; }         
            if(cam == null) { return; }         

            float shiftX = 0;
            Vector2 boundsPos = new Vector2(targetBounds.position.x, targetBounds.position.y);
            Vector2 boundsSize = new Vector2(targetBounds.localScale.x, targetBounds.localScale.y);
            //Limits based on currect position of character bounds ( ie character position ) and bounds size
            float boundsMinX = boundsPos.x - boundsSize.x/2f;
            float boundsMinY = boundsPos.y - boundsSize.y/2f;
            float boundsMaxX = boundsPos.x + boundsSize.x/2f;
            float boundsMaxY = boundsPos.y + boundsSize.y/2f;

            //Figure out what is the area of camera, ie where are camera border in the scene ( 0 is left )
            //AreaZone encapsulates visible area for cameras, once camBounds are outside of that area, stop any movement
            //Once camera should go outside of this area, stop any camera movement in that direction 
            float camBoundsLeft = cam.ScreenToWorldPoint(new Vector3(0, Screen.height/2f, cam.nearClipPlane)).x;
            float camBoundsRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height/2f, cam.nearClipPlane)).x;
            float camBoundsTop = cam.ScreenToWorldPoint(new Vector3(Screen.width/2f, Screen.height, cam.nearClipPlane)).y;
            float camBoundsBottom = cam.ScreenToWorldPoint(new Vector3(Screen.width/2f, 0, cam.nearClipPlane)).y;
           
          
            float areaZoneLeft = cameraZone.position.x - cameraZone.localScale.x/2f;
            float areaZoneRight = cameraZone.position.x + cameraZone.localScale.x/2f;
            float areaZoneTop = cameraZone.position.y + cameraZone.localScale.y/2f;
            float areaZoneBottom = cameraZone.position.y - cameraZone.localScale.y/2f;    

            //Character bounds are more to the left then focus area, move camera, so that character bounds stay inside the focuse area
            //Second check with Cambounds and areaZone makes sure that if camera is at the edge of area, the player bounds and area centering is essentially ignored, because camera needs to respect area border ( it overrides centering player )
            //0.01 is because the check was not working with exact value, essentially if camera bounds ( camera edges ) are out of area zone, stop centering player
            if((boundsMinX < areaLeft) && (camBoundsLeft > (areaZoneLeft+0.01f))) {
                shiftX = boundsMinX - areaLeft;
                //print("Cam 1 " + " boundsMinX " + boundsMinX + " areaLeft " + areaLeft + " shiftX " + shiftX + " camBoundsLeft " + camBoundsLeft + " areaZoneLeft " + areaZoneLeft);
            }
            else if ((boundsMaxX > areaRight) && (camBoundsRight < (areaZoneRight-0.01f))) {
            shiftX = boundsMaxX - areaRight;
               //print("Cam 1 " + " boundsMaxX " + boundsMaxX + " areaRight " + areaRight + " shiftX " + shiftX + " CamBoundsRight " + camBoundsRight + " areaZoneRight " + areaZoneRight);
            }

            //Camera is outside of camera area, stop its movement
            if(camBoundsLeft <= areaZoneLeft && shiftX < 0) {
                shiftX = 0;
                //print("Cam 2 Left");
            }

            if(camBoundsRight >= areaZoneRight && shiftX > 0) {
                shiftX = 0;
                //print("Cam 2 Right");
            }

            //When Camere is completely off the area when new area is spawned, move it to correct positino to respect area zone bounds
            if(camBoundsLeft <= areaZoneLeft) {
                shiftX += (areaZoneLeft - camBoundsLeft);
                //print("Cam 3 " + " camBoundsLeft " + camBoundsLeft + " areaZoneLeft " + areaZoneLeft + " shiftX " + (areaZoneLeft - camBoundsLeft));
            }
             if(camBoundsRight >= areaZoneRight) {
                shiftX += (areaZoneRight - camBoundsRight);
                //print("Cam 3 " + " CamBoundsRight " + camBoundsRight + " areaZoneRight " + areaZoneRight + " shiftX " + (areaZoneRight - camBoundsRight));
            }

            //Move both positions ( left and right based on the shift ), left and right side both have to move same amount, so it stays same size
            areaLeft += shiftX;
            areaRight += shiftX;

            float shiftY = 0;
            
            //character is under the focus area, move focus area down
            if(boundsMinY < areaBottom) {
                shiftY = boundsMinY - areaBottom;               
            }
            else if (boundsMaxY > areaTop) {
                shiftY = boundsMaxY - areaTop;               
            }            

            if(camBoundsTop > areaZoneTop && shiftY > 0) {
                shiftY = 0;
            }

            if(camBoundsBottom < areaZoneBottom && shiftY < 0) {
                shiftY = 0;
            }          
            
            //Move again both bottom and top position of the focus area based on the shifting of Y
            areaTop += shiftY;
            areaBottom += shiftY;        

            //Calculate center to position focus area correctly
            //print(areaLeft + " areas " + areaRight + " , shift " + shiftX);
            center = new Vector2((areaLeft + areaRight) / 2f, (areaTop + areaBottom) / 2f);                             
            velocity = new Vector2(shiftX, shiftY);
        }

        public void Recenter(Transform targetBounds, Vector2 size) {
            areaLeft = targetBounds.position.x - size.x/2f;
            areaRight = targetBounds.position.x + size.x/2f;
            areaBottom = targetBounds.position.y - size.y/2f;
            areaTop = targetBounds.position.y + size.y/2f;           
          
            center = new Vector2((areaLeft + areaRight) / 2, ( areaTop + areaBottom) / 2);           
        }      
       
    }
}
