using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum CameraMode
{
  Tracking,
  Follow
}

/*
 * Rotations:
 *   x -> -yaw
 *   y -> -roll
 *   z -> pitch
 *   in that order, while Unity does z, x, y.
 */
public class Rocket : MonoBehaviour
{
  private enum State
  {
    PadIdle,
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
  public float mass;

  private readonly Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
  private int flightStartIndex;
  private int flightEndIndex;
  private int flightCurrentIndex;
  private float flightStartTime;
  private new Camera camera;
  private new Rigidbody rigidbody;
  private State state = State.PadIdle;

  // Start is called before the first frame update
  private void Start()
  {
    camera = Camera.main;
    rigidbody = GetComponent<Rigidbody>();

    rigidbody.mass = mass;

    Assert.IsNotNull(camera);
    Assert.IsNotNull(rigidbody);

    string[] lines = logFile.text.Split('\n');

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

    SetCameraPosition(transform.position);

    flightStartIndex = values["flightMicros"].FindIndex(value => int.Parse(value) > 0) - 1;

    // Due to the data cutout, we have to end the file early to preserve the downward momentum
    if (logFile.name == "flight3")
    {
      flightEndIndex = 850;
    }
    else
    {
      flightEndIndex = values["flightMicros"].Count - 1;
    }

    flightCurrentIndex = flightStartIndex;
    flightStartTime = Time.time + 1;
  }

  // Update is called once per frame
  private void Update()
  {
    if (state == State.Rigidbody)
    {
      SetCameraPosition(transform.position);

      return;
    }

    if (Time.time >= flightStartTime + float.Parse(values["flightMicros"][flightCurrentIndex]) / 1e6)
    {
      float altitude = float.Parse(values["altitude"][flightCurrentIndex]);
      float yaw = float.Parse(values["yaw"][flightCurrentIndex]) * Mathf.Rad2Deg;
      float pitch = float.Parse(values["pitch"][flightCurrentIndex]) * Mathf.Rad2Deg;
      float roll = float.Parse(values["roll"][flightCurrentIndex]) * Mathf.Rad2Deg;
      float servoY = float.Parse(values["yServo"][flightCurrentIndex]);
      float servoZ = float.Parse(values["zServo"][flightCurrentIndex]);
      float accelerationX = float.Parse(values["aX"][flightCurrentIndex]);
      int internalState = int.Parse(values["state"][flightCurrentIndex]);

      float thrust = accelerationX * mass;

      if (state == State.PoweredFlight)
      {
        ParticleSystem.MainModule mainModule = FIRE.main;
        mainModule.startSpeed = -thrust / 2;
      }

      Vector3 position = new Vector3(transform.position.x, altitude, transform.position.z);

      transform.position = position;

      SetCameraPosition(position);

      Quaternion rotationAngleAxis = Quaternion.AngleAxis(-yaw, Vector3.right) *
                                     Quaternion.AngleAxis(-pitch, Vector3.back) *
                                     Quaternion.AngleAxis(-roll, Vector3.up);

      transform.rotation = rotationAngleAxis;

      motor.transform.localRotation = Quaternion.Euler(-90, 0, 0) *
                                      Quaternion.AngleAxis(servoY, Vector3.back) *
                                      Quaternion.AngleAxis(servoZ, Vector3.right);

      if (internalState <= 2)
      {
        SetState(State.PoweredFlight);
      }
      else if (internalState <= 3)
      {
        SetState(State.Coasting);
      }
      else
      {
        SetState(State.Freefall);
      }

      if (state == State.Freefall && altitude < 0.5f)
      {
        SetState(State.Rigidbody);
      }

      if (flightCurrentIndex <= flightEndIndex)
      {
        flightCurrentIndex++;
      }
      else
      {
        SetState(State.Rigidbody);
      }
    }
  }

  private void SetCameraPosition(Vector3 rocketPosition)
  {
    switch (cameraMode)
    {
      case CameraMode.Follow:
        camera.transform.position = new Vector3(rocketPosition.x + 2f, rocketPosition.y + 1.5f, rocketPosition.z);
        break;
      case CameraMode.Tracking:
        camera.transform.position = new Vector3(10f, 1f, 0f);
        camera.transform.LookAt(rocketPosition);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(cameraMode), $"Unknown camera mode '{cameraMode}'");
    }
  }

  private void SetState(State newState)
  {
    if (state == newState)
    {
      return;
    }

    Debug.Log($"Switching from state '{state}' to '{newState}'");

    state = newState;

    switch (newState)
    {
      case State.PoweredFlight:
        FIRE.Play();
        smokeParticles.Play();
        break;
      case State.Coasting:
        FIRE.Stop();
        smokeParticles.Stop();
        break;
      case State.Rigidbody:
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;

        float lastAltitude = float.Parse(values["altitude"][flightCurrentIndex - 1]);
        float currentAltitude = float.Parse(values["altitude"][flightCurrentIndex]);

        float lastYaw = float.Parse(values["yaw"][flightCurrentIndex - 1]);
        float currentYaw = float.Parse(values["yaw"][flightCurrentIndex]);
        float lastPitch = float.Parse(values["pitch"][flightCurrentIndex - 1]);
        float currentPitch = float.Parse(values["pitch"][flightCurrentIndex]);
        float lastRoll = float.Parse(values["roll"][flightCurrentIndex - 1]);
        float currentRoll = float.Parse(values["roll"][flightCurrentIndex]);

        float dt = float.Parse(values["logDt"][flightCurrentIndex]) / 1e6f;

        rigidbody.velocity = new Vector3(0, (currentAltitude - lastAltitude) / dt, 0);
        rigidbody.angularVelocity = new Vector3(-(currentYaw - lastYaw) / dt, -(currentRoll - lastRoll) / dt, (currentPitch - lastPitch) / dt);
        break;
      case State.Freefall:
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(newState), $"Unknown state '{newState}'");
    }
  }
}