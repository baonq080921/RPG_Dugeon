using System.Collections;
using enemy;
using UnityEngine;

public class EnemySlime : Enemy
{
    public EnemySlimeDeathState enemySlimeDeathState;
    [SerializeField] private int _childAmount;
    [SerializeField] private GameObject _slimeChildPefab;
    private Coroutine _spawnCoroutine;
    [SerializeField] private float _shootOutPowerX = 4f;
    [SerializeField] private float _shootOutPowerYMin = 4f;
    [SerializeField] private float _shootOutPowerYMax = 8f;
    [SerializeField] private bool _enableReformAnimation = true;

    protected override void Awake()
    {
        base.Awake();
        enemySlimeDeathState = new EnemySlimeDeathState(this, stateMachine, "Died");
    }

    protected override void Start()
    {
        base.Start();
        animator.SetBool("HasRecovery", _enableReformAnimation);
    }

    public override void ChangeToDiedState()
    {
        base.ChangeToDiedState();
        stateMachine.ChangeState(enemySlimeDeathState);
    }

    public void CreateChild()
    {
        if (_slimeChildPefab == null || _childAmount <= 0) return;
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = StartCoroutine(SpawnSlimeChild());
    }

    private IEnumerator SpawnSlimeChild()
    {
        while (_childAmount > 0)
        {
            yield return new WaitForSeconds(0.2f);
            GameObject go = Instantiate(_slimeChildPefab);
            var slimeChild = go.GetComponent<EnemySlime>();
            slimeChild.transform.position = transform.position;
            SetUpSlimeChild(slimeChild);
            _childAmount--;
        }
    }

    private void SetUpSlimeChild(EnemySlime enemySlime)
    {
        Vector2 velocity = new Vector2(
            Random.Range(-_shootOutPowerX, _shootOutPowerX),
            Random.Range(_shootOutPowerYMin, _shootOutPowerYMax));
        enemySlime.SetVelocity(velocity);
    }
}
