using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
	//referência do código do player
	private PlayerControl PlayerC;
	//transform do player
	private Transform PlayerTransf;
	
	//componente do nav mesh
	private NavMeshAgent navAgent;
	
	//componente de vida
	private EnemyHealth E_HP;
	
	//enum com todos os states
	private enum State
	{
		Inactive,//fora de combate
		Active,//inicia o combat
		
		Approach,//indo até o player
		Retreat,//indo para longe do player
		Reposition,//anda até uma posição pré-definida
		Melee,//ataque melee
		Ranged,//ataque ranged
		
		Dead,//quando está morto
		
		Reset//reseta a luta e volta pra posição inicial
	};
	
	//state atual
	[SerializeField]
	private State currentState;
	
    private void Start()
    {
        //pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
		//pega o transform do objeto do player
		PlayerTransf = PlayerC.transform;
		
		navAgent = GetComponent<NavMeshAgent>();
		
		E_HP = GetComponent<EnemyHealth>();
		
		OnStart();
    }
	
	//funções adicionais do Start, que podem ser adicionadas depois
	protected virtual void OnStart()
	{
		
	}
	
	//ativa o inimigo e começa o combate
	public void Activate()
	{
		if(currentState == State.Inactive || currentState == State.Reset)
		{
			currentState = State.Active;
		}
	}
	//desativa o inimigo, encerrando o combate
	public void Deactivate()
	{
		if(currentState != State.Inactive && currentState != State.Reset && currentState != State.Dead)
		{
			currentState = State.Reset;
			
			//reseta o HP
			E_HP.ResetHP();
		}
	}
	
    private void FixedUpdate()
	{
		switch(currentState)
		{
			case State.Inactive:
				Inactive();
				break;
			
			case State.Active:
				Active();
				break;
			
			case State.Approach:
				Approach();
				break;
			
			case State.Retreat:
				Retreat();
				break;
			
			case State.Reposition:
				Reposition();
				break;
			
			case State.Melee:
				Melee();
				break;
			
			case State.Ranged:
				Ranged();
				break;
			
			case State.Dead:
				Dead();
				break;
			
			case State.Reset:
				Reset();
				break;
			
			default:
				print("Enemy state machine error.");
				break;
		}
	}
	
	#region states
	protected virtual void Inactive()
	{
		
	}
	
	protected virtual void Active()
	{
		
	}
	
	protected virtual void Approach()
	{
		
	}
	
	protected virtual void Retreat()
	{
		
	}
	
	protected virtual void Reposition()
	{
		
	}
	
	protected virtual void Melee()
	{
		
	}
	
	protected virtual void Ranged()
	{
		
	}
	
	protected virtual void Dead()
	{
		
	}
	
	protected virtual void Reset()
	{
		
	}
	#endregion
}
