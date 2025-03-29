using System;
using ItemSystem;
using Services;
using UnityEngine;

namespace Rounds
{
    public class DateSceneGameManager : BaseGameManager
    {
        private RoundConfig _config;
        private float _currentPositiveProgress;
        private float _currentNegativeProgress;
        
        public float CurrentNegativeProgress
        {
            get => _currentNegativeProgress;
            private set
            {
                _currentNegativeProgress = Math.Clamp(value, 0, 100);
                EventService.Instance.OnDateNegativeProgressUpdate(_currentNegativeProgress);
                if (_currentNegativeProgress >= 100)
                {
                    EndDate();
                }
            }
        }

        public float CurrentPositiveProgress
        {
            get => _currentPositiveProgress;
            private set
            {
                _currentPositiveProgress = Math.Clamp(value, 0, 100);
                if (_currentPositiveProgress <= 100)
                {
                    EventService.Instance.OnDatePositiveProgressUpdate(_currentPositiveProgress);
                }
            }
        }
        
        private void Awake()
        {
            _config = ConfigService.Instance.roundConfig;
            if (_config == null)
            {
                Debug.LogError("No config found for DateSceneGameManager!");
            }
        }

        private void OnEnable()
        {
            EventService.Instance.OnAddPositiveEffect += AddingPositiveProgress;
        }

        private void OnDisable()
        {
            EventService.Instance.OnAddPositiveEffect -= AddingPositiveProgress;
        }

        protected override async void Start()
        {
            base.Start();
            Initialization.Initialize();
            await ItemService.Instance.CreateItem("FirstPerfume");
            InventoryService.Instance.EquipItem("FirstPerfume");
            await ItemService.Instance.CreateItem("RichPerfume");
            InventoryService.Instance.EquipItem("RichPerfume");
            StartDate();
        }
        
        private void Update()
        {
            if (GameState == GameState.Paused)
            {
                return;
            }
        }

        private void StartDate()
        {
            // запускаем заполнение негативного прогресса
            CoroutineService.Instance.RunRepeatingCoroutine(AddingNegativeProgress, 1f, () => GameState != GameState.Running);
            // запускаем действия эффектов у айтемов
            ApplyEffectsOnItems(effect => effect.OnDateStart());
        }
        
        public void EndDate()
        {
            // останавливаем действие эффектов у айтемов
            ApplyEffectsOnItems(effect => effect.OnDateEnd());
            GameState = GameState.Ending;
            Debug.Log("Round date end");
        }
        
        private void AddingPositiveProgress(float value) =>
            CurrentPositiveProgress += value;
        
        private void AddingNegativeProgress() =>
            CurrentNegativeProgress += _config.negativeProgressInSecond;

        private void ApplyEffectsOnItems(Action<IEffect> effectAction)
        {
            var items = InventoryService.Instance.Items;
            foreach (var item in items)
            {
                effectAction?.Invoke(item.Effect);
            }
        }
    }
}