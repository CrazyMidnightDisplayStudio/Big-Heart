using Events.Gameplay;
using Events.UI;
using UI;
using UnityEngine;

namespace Services
{
    public class DateProgressService : Service, IDateProgressService
    {
        public float Positive { get; private set; }
        public float Negative { get; private set; }

        private const float NegativeLimit = 100f;
        private const float PositiveLimit = 100f;

        private readonly IEventService _eventService;

        public DateProgressService(IEventService eventService) : base("DateProgressService")
        {
            _eventService = eventService;
        }

        public void AddPositive(float value)
        {
            Positive = Mathf.Min(Positive + value, PositiveLimit);
            _eventService.Publish(new PositiveProgressEvent(Positive));
            _eventService.Publish(new PositiveSliderUpdateEvent(Positive));
        }

        public void AddNegative(float value)
        {
            Negative = Mathf.Min(Negative + value, NegativeLimit);
            _eventService.Publish(new NegativeProgressEvent(Negative));
            _eventService.Publish(new NegativeSliderUpdateEvent(Negative));
        }

        public override void Dispose() { }
    }
}