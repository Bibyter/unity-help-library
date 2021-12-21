using UnityEngine;
using UnityEngine.SceneManagement;
using Client;

namespace Client
{
    public interface IGameLevel
    {
        int currentIndex { get; }
        int currentNumber { get; }
    }

    public interface ILevelLoader
    {
        void LoadNext();
        void Reload();
    }
}

namespace Client.Services
{
    public sealed class LevelLoopSystem : MonoBehaviour, IGameLevel, ILevelLoader
    {
        [SerializeField] int _tutorialLevelsCount = 2;
        [SerializeField] string[] _levelsName;

        int _currentLevel;

        int IGameLevel.currentIndex => _currentLevel;
        int IGameLevel.currentNumber => _currentLevel + 1;

        private void OnEnable()
        {
            LoadData();
        }

        private void LoadData()
        {
            _currentLevel = PlayerPrefs.GetInt("level", 0);
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("level", _currentLevel);
        }

        public void LoadNextLevel()
        {
            _currentLevel++;
            LoadLevel(_currentLevel);
            SaveData();
        }

        public void ReloadLevel()
        {
            LoadLevel(_currentLevel);
        }

        void LoadLevel(int id)
        {
            var sceneNextIndex = ValidateLevel(id, _levelsName.Length, _tutorialLevelsCount);
            SceneManager.LoadScene(_levelsName[sceneNextIndex]);
        }

        static int ValidateLevel(int lvl, int count, int ignoreLoopLevelsCount)
        {
            if (lvl + 1 > count)
            {
                return ((lvl - ignoreLoopLevelsCount) % (count - ignoreLoopLevelsCount)) + ignoreLoopLevelsCount;
            }
            else
            {
                return lvl;
            }
        }

        void ILevelLoader.LoadNext()
        {
            LoadNextLevel();
        }

        void ILevelLoader.Reload()
        {
            ReloadLevel();
        }
    }
}