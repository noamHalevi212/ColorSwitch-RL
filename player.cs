using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;
                                    //ייבוא כל המחלקות הדרושות לפרוייקט
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.MLAgents.Actuators;
using System;
using Unity.VisualScripting;

public class Player : Agent //יצירת המחלקה שמייצגת את הסוכן במשחק.
{
    public Rigidbody2D rb; //משמש להזזת השחקן– אחראי על התנועה של הכדור.
    public float jump_Height = 10f; // גובה הקפיצה של השחקן בכל פעם שהוא קופץ.
    public SpriteRenderer sr; //הרכיב שאחראי על הצגת הצבע של השחקן.
    public Camera camera; //מצלמה שעוקבת אחרי השחקן, המיקום שלה מתאפס בסוף כל פרק והוא מאותחל להיות קצת מעל למיקום 
    //ההתחלתי של השחקן

    public Color yellow_color;
    public Color blue_color;
    public Color purple_color;  //ארבעה צבעים אפשריים של השחקן והמכשולים.
    public Color pink_color;

    public string current_color; //שומר את הצבע הנוכחי של השחקן (לפי טאג)
    private float startYPosition;
    private Dictionary<GameObject, float> obstacleInitialAngles = new Dictionary<GameObject, float>(); //מילון ששומר זוויות התחלתיות של מכשולים 

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin() //מופעלת בתחילת כל אפיזודה(משחק) – מאפסת את מיקום הכדור, מיקום המכשולים
                                          //מבחינת זווית הסיבוב שלהם הם מתחילים כל משחק מאותה הזווית), מיקום המצלמה ומאתחלת צבע חדש לשחקן
    {
        transform.position = Vector3.zero;
        //rb.linearVelocity = Vector2.zero;
        SetRandom_Color();
        obstacleInitialAngles.Clear();
        camera.transform.position = new Vector3(0, 0, -10); 
    }

    public override void CollectObservations(VectorSensor sensor) //שולחת נתונים מנורמלים על צבעו הנוכחי של השחקן, הצבע שנמצא בכל רגע מול השחקן, 
    {//המרחק של השחקן מהמכשול הקרוב, ומיקום השחקן על ציר הY. נתונים אלו נשלחו בתור הצבים/תצפיות של הסוכן לאימון ב mlagents.
        sensor.AddObservation(transform.position.y/155f);
        sensor.AddObservation(testcolor());
        sensor.AddObservation(RaycastHitDistance()/17f);
        sensor.AddObservation(agentColor());
    }  

    public override void OnActionReceived(ActionBuffers actions) //מקבלת את הפעולה שהרשת החליטה לבצע – .
    {//אם הפעולה היא קפיצה, מבוצעת קפיצה, וניתנים תגמולים בהתאם לגובה. אם לא התקבלה שום פולה ברירת המחדל של השחקן היא לעצור במקום.
        int action = actions.DiscreteActions[0];
        if (action == 1)
        {
            Jump_Ball();
            AddReward(1f); //מתן תגמול חיובי על עלייה. מאחר והשחקן יכול רק לעלות או לעצור בכל פעם שהוא מבצע קפיצה הוא בעצם עולה 
            //ועל כן מקבל תגמול חיובי.
        }

        if (transform.position.y == 154f)
            AddReward(1000f); //"תגמול גבוה מאוד כשהשחקן מגיע לסוף של המשחק "מנצח:
        if (transform.position.y > 155f)
            EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut) //מאפשרת שליטה ידנית כדי לבדוק את המשחק
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    void Jump_Ball() //פונקציה שמבצעת קפיצה לשחקן – מוסיפה לגובה שלו את jump_Height.
    {
        Vector3 pos = transform.position;
        pos.y += jump_Height;
        transform.position = pos;
    }

    void SetRandom_Color() //קובעת לשחקן צבע חדש באקראי מתוך ארבעת הצבעים האפשריים.
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
    }

    private float[] testcolor() //הפונקציה בודקת באמצעות RayCast שפוגעת באובייקט הקרוב ביותר מה הוא בצבע שנמצא בכל פריים מול הסוכן 
    {//ומחזירה מערך של ארבעה תאים עם 0 ו 1 שכל תא מייצג את אחד מן הצבעים, מוסבר בהרחבה בתיק פרוייקט.
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
        }
        return numbers;
    }
    
    private float RaycastHitDistance() //מחזירה את המרחק בין השחקן למכשול הכי קרוב מעליו – בעזרת RayCast שמופעלת כלפי מעלה.
    {

        Vector3 POS = transform.position;
        POS.y = transform.position.y + 1;
        Debug.DrawRay(POS, Vector2.up*100f, Color.red);

        Ray ray = new Ray(POS, Vector2.up * 100f);
        RaycastHit2D hit = Physics2D.Raycast(POS, Vector2.up, 100f);

        if (hit.collider != null)
        {
            return Mathf.Abs(transform.position.y - hit.transform.position.y);
        }
        else
        {
            return -1f;
        }
    }

    private float[] agentColor() //מחזירה מערך של ארבעה תאים עם 1 או 0 לפי הצבע הנוכחי של השחקן .
    {
        
        float[] numbers1 = { 0f, 0f, 0f, 0f };
        
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
        
        return numbers1;
    }
 
    private void OnTriggerEnter2D(Collider2D other) //מתבצעת כשהשחקן פוגע באובייקט אחר –.
    {// אם זה צבע נכון (תואם לצבע שלו) הוא מקבל תגמול, ואם לא – האמשחק נגמר והסוכן נענש
        if (!other.gameObject.CompareTag(current_color) && !other.CompareTag("Color_changer"))
        {
            AddReward(-15); //תגמול שלילי על פגיעה במכשול בצבע שלא תואם את צבע השחקן
            EndEpisode();
        }
        else
        {
            AddReward(40f); //תגמול חיובי על מעבר בהצלחה מכשול (כניסה דרך צבע שתואם את צבע השחקן
        }
    }
}

