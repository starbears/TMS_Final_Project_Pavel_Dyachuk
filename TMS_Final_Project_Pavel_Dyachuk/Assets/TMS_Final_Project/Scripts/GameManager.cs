using Platformer.Common;
using Platformer.Events;
using Platformer.Pickables;
using Platformer.Player;
using UnityEditor;
using UnityEngine;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private PlayerController _player;

        [SerializeField]
        private IntEventChannel _scoreChannel;

        [SerializeField]
        private IntEventChannel _numLivesChannel;

        private int _score;


        private void Start()
        {
            if (_camera.TryGetComponent<Follower>(out var follower))
            {
                follower.Init(_player.transform);
            }

            _player.OnPickableCollected += OnPickableCollected;
        }

        private void OnDestroy()
        {
            _player.OnPickableCollected -= OnPickableCollected;
        }

        private void OnHealthNumLivesChanged(int numLives)
        {
            _numLivesChannel.Publish(numLives);
        }

        private void OnPickableCollected(IPickable pickable)
        {
            if (pickable == null)
            {
                return;
            }

            if (pickable is CherryOfInvulnerability)
            {
                //if (!_player.IsInvulnerable)
                //{
                //    _player.EnableInvulnerability();
                //    pickable.Pick(_player.gameObject);
                //    SoundManager.Sound_Manager.PlayPickCherrySound();
                //    _score += pickable.ScoreIncrement;
                //}
            }
            else
            {
                pickable.Pick(_player.gameObject);
                SoundManager.Sound_Manager.PlayPickGemSound();
                _score += pickable.ScoreIncrement;
            }

            _scoreChannel.Publish(_score);
        }
    }
}