using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.MLAgents.Actuators;
using System;
using Unity.VisualScripting;

public class Player : Agent
{
    public Rigidbody2D rb;
    public float jump_Height = 10f;
    public float maxHeightReward = 10f;
    public float currentHeightReward = 0.5f;
    public SpriteRenderer sr;
    public Camera camera;

    public Color yellow_color;
    public Color blue_color;
    public Color purple_color;
    public Color pink_color;

    public string current_color;
    private float startYPosition;
    //private float maxYPosition;
    private Dictionary<GameObject, float> obstacleInitialAngles = new Dictionary<GameObject, float>();
    public GameObject tester;

   // public GameObject movemementColor;
    private int stepCount = 0;
    private float MaxHitDistance = 0;

    public GameObject[] colorSwitchers;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D לא נמצא! ודא שהרכיב מחובר לאובייקט.");
        }
    }


    public override void OnEpisodeBegin()
    {
        Debug.Log("max"+MaxHitDistance);
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        stepCount = 0;
        SetRandom_Color();
        startYPosition = transform.position.y;
        //maxYPosition = transform.position.y;
        obstacleInitialAngles.Clear();
        camera.transform.position = new Vector3(0, 0, -10);
        for (int i = 0; i < colorSwitchers.Length; i++)
        {
            colorSwitchers[i].SetActive(true);
        }

    }

    /* public override void CollectObservations(VectorSensor sensor)
     {
         sensor.AddObservation(transform.position.y);
         sensor.AddObservation(rb.linearVelocity.y);
         sensor.AddObservation(GetColorIndex());

         string nearestObstacleType = GetNearestObstacleType();
         float distanceToObstacle = GetDistanceToNearestObstacle(nearestObstacleType);
         float obstacleRotation = GetObstacleRotationByType(nearestObstacleType);

         sensor.AddObservation(distanceToObstacle);
         sensor.AddObservation(GetObstacleTypeIndex(nearestObstacleType));
         sensor.AddObservation(obstacleRotation);
         string nearestObstacleType = GetNearestObstacleType();
         float distanceToObstacle = GetDistanceToNearestObstacle(nearestObstacleType);
         sensor.AddObservation(distanceToObstacle);
         sensor.AddObservation(testercolor()); 

         //Debug.Log($"Observations: Y={transform.position.y}, VelY={rb.linearVelocity.y}, Color={GetColorIndex()}, " +
         //          $"Dist={distanceToObstacle}, ObstacleType={GetObstacleTypeIndex(nearestObstacleType)}, Rot={obstacleRotation}");
     }*/

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.y/155f);
        
        sensor.AddObservation(testcolor());
        //Debug.Log("Color Observation: " + GetColorFromArray(testcolor()));
       
        sensor.AddObservation(RaycastHitDistance()/17f);
        


        sensor.AddObservation(agentColor());

        //Debug.Log("Agent Color: " + GetColorFromArray(agentColor()));
        //sensor.AddObservation(rb.linearVelocity.y);

        //sensor.AddObservation(isPositive(rb.linearVelocity.y));
    }






    // עדכון: שימוש ב- float[] במקום ActionBuffers

    private int isPositive(float x)
    {
        if(x > 0)
        {
            return 1;
        }
        if (x < 0)
        {
            return -1;
        }
        return 0;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];
       
        //AddReward(0.01f); // תגמול על לשרוד

        //AddReward(stepCount * 0.5f);

        // MoveChecker(action);
        if (action == 1)
        {
            Jump_Ball();
            AddReward(1f);
        }

        /*if(rb.linearVelocity.y < 0)
        {
            AddReward(isPositive(rb.linearVelocity.y)*5);
        }*/
        

         //if (transform.position.y > maxYPosition)
         //{
         //   AddReward(10f);// (transform.position.y - maxYPosition) * maxHeightReward);
         //    Debug.Log("GOOD REWARD");
         //    maxYPosition = transform.position.y;
         //}

        //AddReward((transform.position.y - startYPosition) * currentHeightReward);

        /*if (transform.position.y < -6)
        {
            AddReward(-100);
        }
        if (transform.position.y < -7)
        {
            AddReward(-100);
        }
        if (transform.position.y < -8)
        {
            AddReward(-100);
        }
      if (transform.position.y < -10f)
        {
            Debug.Log("[GAME OVER] הכדור יצא מהתחום.");
            AddReward(-100f);
            //Debug.Log("BAD REWARD");
            EndEpisode();
        }*/

        if (transform.position.y > 155f)
        {
            //Debug.Log("Win");
            AddReward(1000f);
            //Debug.Log("BAD REWARD");
            EndEpisode();

        }
    }

    /*  public override void OnActionReceived(ActionBuffers actions)
      {
          int action = actions.DiscreteActions[0];
          if (action == 1)
          {
              Jump_Ball();
              AddReward(-0.001f); // עונש קטן על קפיצה כדי למנוע ספאם
          }

          // תגמול על עלייה בגובה (תמריץ לסוכן להמשיך לעלות)
          if (transform.position.y > maxYPosition)
          {
              AddReward((transform.position.y - maxYPosition) * 0.5f);
              maxYPosition = transform.position.y;
          }

          // תגמול על הישרדות
          AddReward(0.005f);

          if (transform.position.y < -5f)
          {
              Debug.Log("[GAME OVER] הכדור יצא מהתחום.");
              AddReward(-1.0f);
              EndEpisode();
          }

          if (transform.position.y > 130f)
          {
              Debug.Log("Win");
              AddReward(10.0f); // במקום 100,000 (ערך קיצוני)
              EndEpisode();
          }
      }*/


    // עדכון: שימוש ב- float[] במקום ActionBuffers
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Debug.Log("hi");
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        //Debug.Log(Input.GetKey(KeyCode.Space) ? 1 : 0);
    }

    void Jump_Ball()
    {
        Vector3 pos = transform.position;
        pos.y += jump_Height;
        transform.position = pos;
        //rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_Height);
    }

    void SetRandom_Color()
    {
        int index = UnityEngine.Random.Range(0, 4);
        if (index == 0)
        {
            sr.color = purple_color;
            current_color = "purple_color";
        }
        else if (index == 1)
        {
            sr.color = yellow_color;
            current_color = "yellow_color";
        }
        else if (index == 2)
        {
            sr.color = blue_color;
            current_color = "blue_color";
        }
        else
        {
            sr.color = pink_color;
            current_color = "pink_color";
        }
        //Debug.Log("[INFO] צבע חדש שנבחר: " + current_color);
    }

    private int GetColorIndex()
    {
        if (current_color == "purple_color") return 0;
        if (current_color == "yellow_color") return 1;
        if (current_color == "blue_color") return 2;
        if (current_color == "pink_color") return 3;
        return -1;
    }
    

    private void Update()
    {
        //Debug.Log(testercolor());
    }
    /*  private int testercolor()
      {
          RaycastHit2D[] ray = Physics2D.RaycastAll(transform.position, Vector2.up);
          for (int i = 0; i < ray.Length; i++)
          {
              if (ray[i].collider.CompareTag("blue_color") || ray[i].collider.CompareTag("purple_color") || ray[i].collider.CompareTag("pink_color") || ray[i].collider.CompareTag("yellow_color"))
              {
                  if (ray[i].collider.CompareTag(current_color))
                  {
                      return 1;
                  }
                  else
                  {
                      return 0;
                  }
              }
          }
          return 0;
      }*/


    private float[] testcolor()
    {
        Vector3 POS = transform.position;
        POS.y = transform.position.y + 1;
        //Debug.DrawRay(POS, Vector2.up*100f, Color.red);
        
        Ray ray = new Ray(POS, Vector2.up * 100f);
        RaycastHit2D hit = Physics2D.Raycast(POS, Vector2.up, 100f);

        float[] numbers = { 0f, 0f, 0f, 0f };
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("blue_color"))
            {
                numbers[0] = 1f;

            }
            else numbers[0] = 0f;
            if (hit.collider.CompareTag("yellow_color"))
            {
                numbers[1] = 1f;
            }
            else numbers[1] = 0f;
            if (hit.collider.CompareTag("pink_color"))
            {
                numbers[2] = 1f;
            }
            else numbers[2] = 0f;
            if (hit.collider.CompareTag("purple_color"))
            {
                numbers[3] = 1f;
            }
            else numbers[3] = 0f;
            //Debug.Log(hit.collider.tag);
        }
        //Debug.Log("object  0:" + numbers[0] + " 1:" + numbers[1] + " 2:" + numbers[2] + " 3:" + numbers[3]);
        //Debug.Log("-------------------------------");
        return numbers;
    }
    

    private float RaycastHitDistance()
    {

        Vector3 POS = transform.position;
        POS.y = transform.position.y + 1;
        Debug.DrawRay(POS, Vector2.up*100f, Color.red);

        Ray ray = new Ray(POS, Vector2.up * 100f);
        RaycastHit2D hit = Physics2D.Raycast(POS, Vector2.up, 100f);

        if (hit.collider != null)
        {
            //Debug.Log("hit.distbance" + Mathf.Abs(transform.position.y - hit.transform.position.y));
            // מחזיר את המרחק עד האובייקט שפגענו בו
            if(MaxHitDistance< Mathf.Abs(transform.position.y - hit.transform.position.y))
            {
                MaxHitDistance = Mathf.Abs(transform.position.y - hit.transform.position.y);
            }
            
            return Mathf.Abs(transform.position.y - hit.transform.position.y);




        }
        else
        {
            // אם לא פגע בכלום — מחזיר ערך שלילי (או ערך שתבחר)
            return -1f;
        }
    }



    private float[] agentColor()
    {
        
        float[] numbers1 = { 0f, 0f, 0f, 0f };
        //Debug.Log(current_color);
        
            if (current_color=="blue_color")
            {
                numbers1[0] = 1f;
            }
            if (current_color == "yellow_color")
            {
                numbers1[1] = 1f;
            }
            if (current_color == "pink_color")
            {
                numbers1[2] = 1f;
            }
            if (current_color =="purple_color" )
            {
                numbers1[3] = 1f;
            }
        //Debug.Log("agent  0:" + numbers1[0] + " 1:" + numbers1[1] + " 2:" + numbers1[2] + " 3:" + numbers1[3]);
        //Debug.Log("-------------------------------");
        return numbers1;
    }
 
    private string GetColorFromArray(float[] array)
    {
     

        if (array[0] == 1f)
            return "blue_color";
        else if (array[1] == 1f)
            return "yellow_color";
        else if (array[2] == 1f)
            return "pink_color";
        else if (array[3] == 1f)
            return "purple_color";
        else
            return "לא ידוע";
    }



    /* private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Color_changer"))
         {
             SetRandom_Color();
             Destroy(other.gameObject);
             return;
         }

         if (!other.gameObject.CompareTag(current_color) && !other.CompareTag("Color_changer"))
         {
             Debug.Log("[GAME OVER] ");
             AddReward(-30f);  // עונש על כישלון
             EndEpisode();
         }
         else
         {
             Debug.Log("succses");
             AddReward(100f);  // **תגמול גבוה להצלחת מעבר מכשול**
         }
     }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Color_changer"))
        {
            SetRandom_Color();
            other.gameObject.SetActive(false);
            return;
        }

        if (!other.gameObject.CompareTag(current_color) && !other.CompareTag("Color_changer"))
        {
            //Debug.Log("[GAME OVER] פגעת בצבע הלא נכון.");
            AddReward(-15);
            EndEpisode();
        }
        else
        {
            //Debug.Log("[SUCCESS] הכדור עבר במכשול הנכון!");
            AddReward(40f);


        }
    }




    /// <summary>
    /// מחזירה את סוג המכשול הקרוב ביותר שנמצא **מעל השחקן**
    /// </summary>
    private string GetNearestObstacleType()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None); // מחליף את השיטה הישנה
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        string nearestObstacleType = "None";  // ברירת מחדל אם לא נמצא מכשול

        foreach (GameObject obj in allObjects)
        {
            // נוודא שהאובייקט **אינו השחקן** ושיש לו Tag שהוא אחד מארבעת סוגי המכשולים
            if (obj != this.gameObject && (obj.CompareTag("small") || obj.CompareTag("medium") || obj.CompareTag("plus") || obj.CompareTag("large")))
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);

                // רק אם המכשול נמצא מעל השחקן
                if (obj.transform.position.y > transform.position.y && distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                    nearestObstacleType = obj.tag; // שומר את סוג המכשול לפי ה-Tag
                }
            }
        }

        return nearestObstacleType;
    }

    private float GetDistanceToNearestObstacle(string obstacleType)
    {
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag(obstacleType);
        float minDistance = Mathf.Infinity;
        float distanceToNearest = -1f; // ערך ברירת מחדל אם אין מכשול מתאים

        foreach (GameObject obstacle in allObstacles)
        {
            float distance = obstacle.transform.position.y - transform.position.y;

            // נבדוק רק מכשולים שנמצאים מעל השחקן
            if (distance > 0 && distance < minDistance)
            {
                minDistance = distance;
                distanceToNearest = distance;
            }
        }

        return distanceToNearest; // אם לא נמצא מכשול מתאים, נחזיר -1
    }


    /// <summary>
    /// ממיר את סוג המכשול למספר כדי להכניס לתוך מערכת ה-ML של Unity
    /// </summary>
    private int GetObstacleTypeIndex(string obstacleType)
    {
        if (obstacleType == "small") return 0;
        if (obstacleType == "medium") return 1;
        if (obstacleType == "plus") return 2;
        if (obstacleType == "large") return 3;
        return -1; // אם לא נמצא מכשול מתאים
    }


    private float GetObstacleRotationByType(string obstacleType)
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(obstacleType);
        if (obstacles.Length == 0)
            return 0f; // אם אין מכשולים מהסוג הזה, נחזיר זווית 0

        // נבחר מכשול אחד (ראשון מהרשימה) כדי לבדוק את הזווית שלו
        return obstacles[0].transform.rotation.eulerAngles.z;
    }

    /*private void MoveChecker(int x)
    {
        if (x > 0f)
        {
            movemementColor.GetComponent<SpriteRenderer>().material.color = Color.green;
        }
        else
        {
            movemementColor.GetComponent<SpriteRenderer>().material.color = Color.red;
        }
    }*/

}

