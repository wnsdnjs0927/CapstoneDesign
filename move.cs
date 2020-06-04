using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{

    public float speed;      // 캐릭터 움직임 스피드.
    public float jumpSpeed; // 캐릭터 점프 힘.
    public float gravity;    // 캐릭터에게 작용하는 중력.
    private bool crouch;    // 캐릭터가 앉았는가

    private CharacterController controller; // 현재 캐릭터가 가지고있는 캐릭터 컨트롤러 콜라이더.
    private Vector3 MoveDir;
    private Animator anime;
    public Camera fpsCam;

    // Start is called before the first frame update
    void Start()
    {
        crouch = false;
        speed = 3.0f;
        jumpSpeed = 6.0f;
        gravity = 20.0f;

        MoveDir = Vector3.zero;
        controller = GetComponent<CharacterController>();
        anime = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 캐릭터가 땅에 있는가?
        if (controller.isGrounded)
        {
            // 위, 아래 움직임 셋팅. 
            MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // 벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 변환한다.
            MoveDir = transform.TransformDirection(MoveDir);

            // 스피드 증가.
            MoveDir *= speed;

            // 캐릭터 점프
            if (Input.GetButton("Jump") && !crouch)
                MoveDir.y = jumpSpeed;

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                anime.SetBool("Crouch Walk", true);
                anime.SetBool("Walk", true);
            }
            else
            {
                anime.SetBool("Crouch Walk", false);
                anime.SetBool("Walk", false);
            }

            anime.SetBool("Jump", false);
        }
        else
        {
            anime.SetBool("Jump", true);
        }

        if (Input.GetKeyDown(KeyCode.C))
            crouch = !crouch;

        if (crouch)
            anime.SetBool("Stand", false);
        else
            anime.SetBool("Stand", true);

        // 캐릭터에 중력 적용.
        MoveDir.y -= gravity * Time.deltaTime;

        // 캐릭터 움직임.
        controller.Move(MoveDir * Time.deltaTime);

        RotCtrl();

    }
    void RotCtrl()
    {
        float rotx = Input.GetAxis("Mouse Y") * speed;
        float rotY = Input.GetAxis("Mouse X") * speed;

        this.transform.localRotation *= Quaternion.Euler(0, rotY, 0);
        fpsCam.transform.localRotation *= Quaternion.Euler(-rotx, 0, 0);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Equals("Stair"))
        {
            Animator anime = other.transform.root.gameObject.GetComponent<Animator>();
            anime.SetBool("Open", false);
        }
    }
}
