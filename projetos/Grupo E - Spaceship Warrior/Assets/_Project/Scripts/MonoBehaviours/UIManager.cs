using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceshipWarrior
{
    public sealed class UIManager : MonoBehaviour
    {
        private const string CurrentWaveTextFormat = "Current Wave: {0}";
        private const string ScoreTextFormat = "Score: {0}";

        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _displayPanel;
        [SerializeField] private Slider _healthBar;
        [SerializeField] private TextMeshProUGUI _currentWaveLabel;
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        private GameObject _activePanel;

        public void UpdateHealthBar(float value)
        {
            _healthBar.value = value;
        }

        public void UpdateCurrentWave(uint value)
        {
            _currentWaveLabel.text = string.Format(CurrentWaveTextFormat, value);
        }

        public void UpdateScore(uint value)
        {
            _scoreLabel.text = string.Format(ScoreTextFormat, value);
        }

        public void ShowStartPanel()
        {
            SetActivePanel(_startPanel);
        }

        public void ShowGameOverPanel()
        {
            SetActivePanel(_gameOverPanel);
        }

        public void HideActivePanel()
        {
            _displayPanel.SetActive(true);
            _activePanel.SetActive(false);
            _activePanel = null;
        }

        private void SetActivePanel(GameObject value)
        {
            _displayPanel.SetActive(false);

            if (_activePanel != null)
            {
                _activePanel.SetActive(false);
            }

            _activePanel = value;
            _activePanel.SetActive(true);
        }
    }
}
