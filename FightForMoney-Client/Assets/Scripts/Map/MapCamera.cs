using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MapCamera : MonoBehaviour {
    private float INTERVAL_X_PER = 1f / 40f;
    private float INTERVAL_Y_PER = 1f / 40f;
    private float SCALE_SPEED = 20f;
    private float MOVE_SPEED = 1f;
    private float ROTATE_SPEED = 1f;
    private float HEIGHT = 10f;
    private float HEIGHT_MIN = 3f;
    private float ROTATION_X = 45f;
    private float ROTATION_Y = 45f;

    private MapEditor map_editor;
    private GameObject camera_obj;
    private Vector3 last_pos;

    private void Awake()
    {
        this.map_editor = GameObject.Find("MapEditor").GetComponent<MapEditor>();
        this.camera_obj = transform.Find("Camera").gameObject;
    }

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.eulerAngles = new Vector3(0, ROTATION_Y, 0);
        this.camera_obj.transform.localPosition = new Vector3(0, HEIGHT, -HEIGHT/Mathf.Tan(ROTATION_X/180*Mathf.PI));
        this.camera_obj.transform.localEulerAngles = new Vector3(ROTATION_X, 0, 0);
    }

    private void LateUpdate()
    {
        if (this.map_editor.IsActive())
        {
            this.CheckMove();
            this.CheckRotate();
            this.CheckScale();
            this.CheckLegal();
        }
    }

    private void CheckMove()
    {
        if (Input.GetMouseButton(1)) return;

        float interval_x = 0;
        float interval_y = 0;

        if (Input.mousePosition.x < Screen.width * INTERVAL_X_PER)
        {
            interval_x = Input.mousePosition.x - Screen.width * INTERVAL_X_PER;
            interval_x = Mathf.Max(interval_x, -Screen.width * INTERVAL_X_PER);
        }

        if (Input.mousePosition.x > Screen.width * (1 - INTERVAL_X_PER))
        {
            interval_x = Input.mousePosition.x - Screen.width * (1 - INTERVAL_X_PER);
            interval_x = Mathf.Min(interval_x, Screen.width * INTERVAL_X_PER);
        }
        
        if (Input.mousePosition.y < Screen.height * INTERVAL_Y_PER)
        {
            interval_y = Input.mousePosition.y - Screen.height * INTERVAL_Y_PER;
            interval_y = Mathf.Max(interval_y, -Screen.height * INTERVAL_Y_PER);
        }

        if (Input.mousePosition.y > Screen.height * (1 - INTERVAL_Y_PER))
        {
            interval_y = Input.mousePosition.y - Screen.height * (1 - INTERVAL_Y_PER);
            interval_y = Mathf.Min(interval_y, Screen.height * INTERVAL_Y_PER);
        }

        float move_speed = this.camera_obj.transform.position.y / HEIGHT * MOVE_SPEED;
        transform.Translate(interval_x / Screen.width * move_speed, 0, interval_y / Screen.height * move_speed);
    }

    private void CheckRotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            this.last_pos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.last_pos = Vector3.zero;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 vector = Input.mousePosition - this.last_pos;
            float rotate_y = vector.y / Screen.height * 180;
            transform.Rotate(0, rotate_y, 0, Space.World);
            this.last_pos = Input.mousePosition;
        }
    }

    private void CheckScale()
    {
        float scale_value = Input.GetAxisRaw("Mouse ScrollWheel") * SCALE_SPEED;
        this.camera_obj.transform.Translate(0, 0, scale_value);
    }

    private void CheckLegal()
    {
        float start_bound_x = -this.map_editor.GetMap().GetWidth()/2;
        float end_bound_x = this.map_editor.GetMap().GetWidth()*3/2;
        float start_bound_z = -this.map_editor.GetMap().GetHeight() / 2;
        float end_bound_z = this.map_editor.GetMap().GetHeight() * 3 / 2;
        float start_bound_y = HEIGHT_MIN;
        float end_bound_y = Mathf.Sqrt(Mathf.Pow(end_bound_x - start_bound_x, 2) + Mathf.Pow(end_bound_z - start_bound_z, 2));

        Vector3 pos = transform.position;
        pos.x = pos.x < start_bound_x ? start_bound_x : pos.x;
        pos.x = pos.x > end_bound_x ? end_bound_x : pos.x;
        pos.z = pos.z < start_bound_z ? start_bound_z : pos.z;
        pos.z = pos.z > end_bound_z ? end_bound_z : pos.z;
        transform.position = pos;

        Vector3 camera_pos = this.camera_obj.transform.localPosition;
        if(camera_pos.y < start_bound_y)
        {
            camera_pos.y = start_bound_y;
            camera_pos.z = -start_bound_y / Mathf.Tan(this.camera_obj.transform.localEulerAngles.x / 180 * Mathf.PI);
        }

        if (camera_pos.y > end_bound_y)
        {
            camera_pos.y = end_bound_y;
            camera_pos.z = -end_bound_y / Mathf.Tan(this.camera_obj.transform.localEulerAngles.x / 180 * Mathf.PI);
        }
        this.camera_obj.transform.localPosition = camera_pos;
    }
}
