using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public enum CameraMode
{
  Tracking,
  Follow
}

public class Rocket : MonoBehaviour
{
  private enum State
  {
    //PadIdle,
    PoweredFlight,
    Coasting,
    Freefall,
    Rigidbody
  }

  public CameraMode cameraMode;
  public TextAsset logFile;
  public ParticleSystem FIRE;
  public ParticleSystem smokeParticles;
  public GameObject motor;

  private Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
  private int flightStartIndex;
  private int flightCurrentIndex;
  
  private float flightStartTime;
  private new Camera camera;
  private new Rigidbody rigidbody;
  private State state;

  // Start is called before the first frame update
  private void Start()
  {
    camera = Camera.main;
    rigidbody = GetComponentInChildren<Rigidbody>();

    Assert.IsNotNull(camera);
    Assert.IsNotNull(rigidbody);

    string data;
    if (logFile == null) {
            data = File.ReadAllText(PlayerPrefs.GetString("filepath"));
            print(data);
    }
    else
        data = logFile.text;




    string[] lines = data.Split('\n');

    string[] fields = lines[0].Split(',');

    for (int i = 1; i < lines.Length; i++)
    {
      string[] uwu = lines[i].Split(',');

      for (int j = 0; j < uwu.Length; j++)
      {
        if (!values.ContainsKey(fields[j]))
        {
          values.Add(fields[j], new List<string>());
        }

        values[fields[j]].Add(uwu[j]);
      }
    }

    Vector3 rocketPosition = transform.position;

    if (cameraMode == CameraMode.Follow)
    {
      camera.transform.position = new Vector3(rocketPosition.x + 2f, rocketPosition.y + 1.5f, rocketPosition.z);
    }
    else if (cameraMode == CameraMode.Tracking)
    {
      camera.transform.position = new Vector3(10f, 1f, 0f);
      camera.transform.LookAt(rocketPosition);
    }

    flightStartIndex = values["flightMicros"].FindIndex(value => int.Parse(value) > 0) - 1;
    flightCurrentIndex = flightStartIndex;
    flightStartTime = Time.time + 1;
  }

  // Update is called once per frame
  private void Update()
  {
    if (Time.time >= flightStartTime + float.Parse(values["flightMicros"][flightCurrentIndex]) / 1e6)
    {
      Vector3 position;

      if (state == State.Rigidbody)
      {
        position = transform.position;

        if (cameraMode == CameraMode.Follow)
        {
          camera.transform.position = new Vector3(position.x + 2f, position.y + 1.5f, position.z);
        }
        else if (cameraMode == CameraMode.Tracking)
        {
          camera.transform.position = new Vector3(10f, 1f, 0f);
          camera.transform.LookAt(transform.position);
        }

        return;
      }

      float altitude = float.Parse(values["altitude"][flightCurrentIndex]);
      float yaw = float.Parse(values["yaw"][flightCurrentIndex]) * Mathf.Rad2Deg;
      float pitch = float.Parse(values["pitch"][flightCurrentIndex]) * Mathf.Rad2Deg;
      float roll = float.Parse(values["roll"][flightCurrentIndex]) * Mathf.Rad2Deg;
      float servoY = float.Parse(values["yServo"][flightCurrentIndex]);
      float servoZ = float.Parse(values["zServo"][flightCurrentIndex]);
      float accelerationX = float.Parse(values["aX"][flightCurrentIndex]);
      int internalState = int.Parse(values["state"][flightCurrentIndex]);

      float thrust = accelerationX * 0.6f;

      if (state == State.PoweredFlight)
      {
        ParticleSystem.MainModule mainModule = FIRE.main;
        mainModule.startSpeed = -thrust / 2;
      }
      else
      {
        FIRE.Stop();
        smokeParticles.Stop();
      }

      position = new Vector3(transform.position.x, altitude, transform.position.z);

      transform.position = new Vector3(position.x, position.y + 0.5f, position.z);

      if (cameraMode == CameraMode.Follow)
      {
        camera.transform.position = new Vector3(position.x + 2f, position.y + 1.5f, position.z);
      }
      else if (cameraMode == CameraMode.Tracking)
      {
        camera.transform.position = new Vector3(10f, 1f, 0f);
        camera.transform.LookAt(transform.position);
      }

      transform.rotation = Quaternion.AngleAxis(-yaw, Vector3.right) *
                           Quaternion.AngleAxis(-pitch, Vector3.back) *
                           Quaternion.AngleAxis(-roll, Vector3.up);

      motor.transform.localRotation = Quaternion.Euler(-90, 0, 0) *
                                      Quaternion.AngleAxis(servoY, Vector3.back) *
                                      Quaternion.AngleAxis(servoZ, Vector3.right);

      if (internalState <= 2)
      {
        state = State.PoweredFlight;
      }
      else if (internalState <= 3)
      {
        state = State.Coasting;
      }
      else
      {
        state = State.Freefall;
      }

      if (state == State.Freefall && altitude < 0.5f)
      {
        state = State.Rigidbody;

        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
      }

      if (flightCurrentIndex < values["altitude"].Count - 1)
      {
        flightCurrentIndex++;
      }
      else
      {
        state = State.Rigidbody;

        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
      }
    }
  }
}