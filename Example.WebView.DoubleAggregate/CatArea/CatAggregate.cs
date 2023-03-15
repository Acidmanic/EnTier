using System;
using System.Collections.Generic;
using EnTier.EventSourcing;
using EnTier.EventSourcing.Attributes;
using EnTier.Utility;
using Example.WebView.DoubleAggregate.CatArea.Events;

namespace Example.WebView.DoubleAggregate.CatArea
{
    public class CatAggregate : AggregateBase<Cat, ICatEvent, long>
    {
        protected override void ManipulateState(ICatEvent e)
        {
            if (e is AdoptedEvent ae)
            {
                CurrentState.Id = ae.StreamId;
                CurrentState.Name = ae.Name;
                CurrentState.AllNames = new List<string> { ae.Name };
                CurrentState.Sterilized = false;
                CurrentState.Weight = ae.Weight;
                CurrentState.LivesOnStreet = false;
            }

            if (e is FattenedEvent f)
            {
                CurrentState.Weight += f.HowMuch;
            }
            
            if (e is LostWeight l)
            {
                CurrentState.Weight -= l.HowMuch;
            }

            if (e is SterilizedEvent)
            {
                CurrentState.Sterilized = true;
            }

            if (e is ReNamedEvent r)
            {
                CurrentState.Name = r.Name;
                
                CurrentState.AllNames.Add(r.Name);
            }

            CurrentState.LastUpdate = new DateTime(e.Timestamp);
        }


        [NoStreamIdApi]
        public void Adopt(string name, double currentWeight)
        {
            if (!this.IsPristine())
            {
                throw new CatException("You can not adopt a cat which is already adopted.");
            }

            var idGenerator = new UniqueIdGenerator<long>();

            var sid = idGenerator.Generate();

            this.Initialize(sid);

            ActionTaken(new AdoptedEvent
            {
                StreamId = sid,
                Name = name,
                Timestamp = DateTime.Now.Ticks,
                Weight = currentWeight
            });
        }


        public void WeightChange(double delta)
        {
            if (delta < 0)
            {
                ActionTaken(new LostWeight
                    {
                        Timestamp = DateTime.Now.Ticks,
                        HowMuch = -delta
                    }
                );
            }

            if (delta > 0)
            {
                ActionTaken(new FattenedEvent
                    {
                        Timestamp = DateTime.Now.Ticks,
                        HowMuch = delta
                    }
                );
            }
        }

        public void Sterilize()
        {
            if (CurrentState.LivesOnStreet)
            {
                throw new CatException(CurrentState, "You MUST not Sterilize a cat which is living on " +
                                                     "the streets; You should adopt them first.");
            }
            ActionTaken(new SterilizedEvent());
        }

        public void ReName( string name)
        {
            if (CurrentState.LivesOnStreet)
            {
                throw new CatException(CurrentState, "You can not add extra name for a cat that you dont own!");
            }
            
            ActionTaken(new ReNamedEvent
            {
                Name = name,
                Timestamp = DateTime.Now.Ticks
            });
        }
    }
}