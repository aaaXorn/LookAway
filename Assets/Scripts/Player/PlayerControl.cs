using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
	//referência global do código do personagem
	public static PlayerControl Instance {get; set;}
	
	//referência do joystick de movimento
	private MovementJoystick MoveJ;
	
	private enum State
	{
		Free,
		Attack
	};
	
	[SerializeField]
	private State currentState;
	
	[Header("Movement")]
	
    public Rigidbody rdb;
    public Animator anim;
    private Vector3 movaxis, turnaxis;
    public GameObject currentCamera;
	public float movespeed = 5;
    public float jumpspeed = 8;
    public float gravity = 20;
	
    public float jumptime;
    private float flyvelocity = 3;
    public GameObject wing;
    public Transform rightHandObj, leftHandObj;
    public bool jumpbtn = false;
    public bool jumpbtndown = false;
    private bool jumpbtnrelease = false;
    private GameObject closeThing;
    private float weight;
	
	public bool attackbtndown = false;
	
	[Header("Attacks")]
	
	//ID do hit, usado pra mesma hitbox não acertar várias vezes
	private int hit_id;
	private string atk_type;//tipo do ataque
	private int atk_hits;//número de hits do ataque
	//dano, duração e tamanho do ataque
	private int[] atk_dmg = new int[5], atk_duration = new int[5], atk_size = new int[5];
	//se o ataque pode ser cancelado (geralmente após um hit)
	private bool atk_cancel;
	//se o jogador está atacando
	private bool attacking;
	
	[System.Serializable]
	//informações dos tipos de ataque
	public class Attack
	{
		[Tooltip("Attack's name")]
		public string id;
		
		[Tooltip("Attack properties")]
		public string type;
		[Tooltip("How many hits the attack has")]
		public int hit_count;
		
		[Tooltip("Attack damage per hit")]
		public int[] dmg;
		[Tooltip("Hitbox duration in frames (24 FPS physics)")]
		public int[] duration;
		[Tooltip("Hitbox size")]
		public int[] size;
	}
	
	//lista com os ataques
	public List<Attack> AtkList;
	//dicionário dos ataques
	//usado para localizar os ataques por string
	public Dictionary<string, Attack> AtkDictionary;
	
	void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
		
		//cria o dicionário de ataques
		AtkDictionary = new Dictionary<string, Attack>();
		foreach(Attack atk in AtkList)
		{
			AtkDictionary.Add(atk.id, atk);
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
		//pega o script de movimento com joystick
		if(MovementJoystick.Instance != null) MoveJ = MovementJoystick.Instance;
		else print("MovementJoystick Instance not found.");
		
        if (SceneManager.GetActiveScene().name.Equals("Land"))
        {
            if (PlayerPrefs.HasKey("OldPlayerPosition"))
            {
                print("movendo "+ PlayerPrefsX.GetVector3("OldPlayerPosition"));
                transform.position = PlayerPrefsX.GetVector3("OldPlayerPosition");
               
            }
        }
        currentCamera = Camera.main.gameObject;
       
    }
    private void Update()
    {
		#if UNITY_EDITOR
        if(Input.GetButtonDown("Jump"))
        {
            jumpbtn = true;
            jumpbtndown = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumpbtn = false;
            jumptime = 0;
        }
		if(Input.GetButtonDown("Fire1"))
		{
			attackbtndown = true;
		}
		//movaxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		#endif
		
		//input de pulo está no script JumpButton
		//input de ataque está no script AttackButton
		
		//movimento com o joystick
		//.normalized garante que o jogador sempre ande na mesma velocidade
		movaxis = new Vector3(MoveJ.Horizontal, 0, MoveJ.Vertical).normalized;
    }

    void FixedUpdate()
    {
		switch(currentState)
		{
			//movimentação e pulo
			case State.Free:
				#region free
				//define a direção relativa que o player vai andar
				Vector3 relativedirection = currentCamera.transform.TransformVector(movaxis);
				relativedirection = new Vector3(relativedirection.x, jumptime, relativedirection.z);
				//mesma coisa só que sem a coordenada Y
				Vector3 relativeDirectionWOy = relativedirection;
				relativeDirectionWOy = new Vector3(relativedirection.x,0, relativedirection.z);
				
				//* 5 / movespeed mantém a animação do jeito que tava antes de aumentar o movespeed
				anim.SetFloat("Speed", rdb.velocity.magnitude * 5 / movespeed);
				
				//movimento com o glider
				if (wing.activeSelf)
				{
					float velocity = Mathf.Abs(rdb.velocity.x)+ Mathf.Abs(rdb.velocity.z);
					velocity = Mathf.Clamp(velocity, 0, 2 * movespeed);

					rdb.AddRelativeForce(new Vector3(0, velocity * 120, 1000));
					
					 Vector3 movfly = new Vector3(Vector3.forward.x* flyvelocity, 0, Vector3.forward.z* flyvelocity);

					 float angz = Vector3.Dot(transform.right, Vector3.up);
					 float angx = Vector3.Dot(transform.forward, Vector3.up);
					 movfly = new Vector3(movaxis.z+ angx*2, -angz, -movaxis.x- angz);

					 transform.Rotate(movfly);

					 wing.transform.localRotation = Quaternion.Euler(0, 0, angz*50);


					 flyvelocity -= angx*0.01f;
					 flyvelocity = Mathf.Lerp(flyvelocity, 3, Time.fixedDeltaTime);
					 flyvelocity = Mathf.Clamp(flyvelocity, 0, movespeed);
					 
				}
				//movimento sem o glider
				else
				{
					rdb.velocity = relativeDirectionWOy * movespeed + new Vector3(0,rdb.velocity.y,0);
					//rdb.AddForce(relativeDirectionWOy * 1000);
					Quaternion rottogo = Quaternion.LookRotation(relativeDirectionWOy * 2 + transform.forward);
					transform.rotation = Quaternion.Lerp(transform.rotation, rottogo, Time.fixedDeltaTime * 50);
				}
				
				//se o botão de ataque foi pressionado
				if (attackbtndown)
				{
					anim.SetTrigger("PunchA");
					
					currentState = State.Attack;
				}
				
				//checa se da pra pular
				RaycastHit hit;
				if (Physics.Raycast(transform.position-(transform.forward*0.1f)+transform.up*0.3f, Vector3.down,out hit, 1000))
				{
					anim.SetFloat("JumpHeight", hit.distance);
					
					//deixa o jogador começar o pulo / hold jump
					if (hit.distance < 0.5f && jumpbtn)
					{
						jumptime = 0.25f;
					}
					
					//ativa as asas
					if (hit.distance > 0.5f && jumpbtndown && !wing.activeSelf)
					{
						wing.SetActive(true);
						jumpbtndown = false;
					}
					//desativa as asas
					else if (hit.distance > 0.5f && jumpbtndown && wing.activeSelf)
					{
					   wing.SetActive(false);
					}
				}
				
				//pulo
				if (jumpbtn)
				{
					jumptime -= Time.fixedDeltaTime;
					jumptime = Mathf.Clamp01(jumptime);
					rdb.AddForce(Vector3.up * jumptime * jumpspeed);
				}
				#endregion
				break;
			
			//ataque
			case State.Attack:
				#region attack
				//propriedades do ataque
				if(attacking)
				{
					
				}
				
				//se o ataque pode ser cancelado
				if(atk_cancel)
				{
					//se o botão de ataque foi pressionado
					if (attackbtndown)
					{
						anim.SetTrigger("PunchA");
					}
					
					//pulo
					else if (jumpbtn)
					{
						//deixa o jogador pular, mesmo no ar (eu acho talvez)
						jumptime = 0.25f;
						
						//volta pro estado normal
						currentState = State.Free;
					}
				}
				#endregion
				break;
		}
		
		//reseta os button down
        jumpbtndown = false;
		attackbtndown = false;
    }


    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (wing.activeSelf)
        {

            if (rightHandObj != null)
            {
               
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);

                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);

            }
        }

        if (closeThing)
        {
            //calcula a direcao do ponto de toque para a personagem
            Vector3 handDirection = closeThing.transform.position - transform.position;
            //verifica se o objeto ta na frente do personagem >0
            float lookto = Vector3.Dot(handDirection.normalized, transform.forward);
            //calcula e interpola o peso pela formula (l*3)/distancia^3
            weight=Mathf.Lerp(weight,(lookto*3 / (Mathf.Pow(handDirection.magnitude,3))),Time.fixedDeltaTime*2);
           
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, closeThing.transform.position + transform.right * 0.1f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.identity);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, closeThing.transform.position- transform.right*0.1f);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.identity);

            if (weight <= 0)
            {
                Destroy(closeThing);
            }
           
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        wing.SetActive(false);
        if (collision.transform.position.y > transform.position.y + .05f) {
            if(!closeThing)
            closeThing = new GameObject("Handpos");

            weight = 0;
            closeThing.transform.parent = collision.gameObject.transform;
            closeThing.transform.position= collision.GetContact(0).point;

        }

    }
	
    private void OnCollisionExit(Collision collision)
    {
		
    }
	
	#region attacks
	//define o dano, duração, tamanho e tipo da hitbox por ID
	void AnimHit(string id)
	{
		//propriedades do ataque
		Attack atk = AtkDictionary[id];
		
		atk_hits = atk.hit_count;
		
		for(int i = 0; i < atk_hits; i++)
		{
			atk_dmg[i] = atk.dmg[i];
			atk_duration[i] = atk.duration[i];
			atk_size[i] = atk.size[i];
		}
		
		//inicia o ataque
		attacking = true;
	}
	
	//volta pro State Free
	void AnimFree()
	{
		currentState = State.Free;
		
		attacking = false;
		atk_cancel = false;
	}
	
	
	#endregion
}
