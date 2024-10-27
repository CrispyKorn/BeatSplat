using UnityEngine;

public class PlayerResizePowerUp : PowerUp
{
    [Tooltip("Multiplier for paddle size.")]
    [SerializeField, Range(0f, 2f)] private float _sizeMultiplier = 1f;

    private Transform _paddle;

    new private void Awake()
    {
        base.Awake();

        _paddle = Locator.Instance.Paddle.PaddleObj.transform;
    }

    public override void ActivatePowerUp()
    {
        _paddle.localScale = new Vector3(_paddle.localScale.x * _sizeMultiplier, _paddle.localScale.y, 1f);
    }

    public override void DeactivatePowerUp()
    {
        _paddle.localScale = new Vector3(_paddle.localScale.x * (1f / _sizeMultiplier), _paddle.localScale.y, 1f);
    }
}
