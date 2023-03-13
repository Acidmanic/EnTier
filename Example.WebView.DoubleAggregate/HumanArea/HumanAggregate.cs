using System;
using System.Collections.Generic;
using EnTier.EventSourcing;
using EnTier.EventSourcing.Attributes;
using Example.WebView.DoubleAggregate.HumanArea.Events;

namespace Example.WebView.DoubleAggregate.HumanArea
{
    public class HumanAggregate:AggregateBase<HumanArea.Human,IHumanEvent,Guid>
    {
        protected override void ManipulateState(IHumanEvent e)
        {
            if (e is IntroducedHumanEvent ie)
            {
                CurrentState.Id = ie.Id;
                CurrentState.Job = ie.Job;
                CurrentState.Name = ie.Name;
                CurrentState.Surname = ie.Surname;
                CurrentState.Description = "";
                CurrentState.MedicalHistory = new List<string>();
            }

            if (e is TrustedHumanEvent te)
            {
                CurrentState.Description += $"Good at {te.SuperPower}, But {te.Weakness}... Not so much.";
                
                CurrentState.IsTrusted = true;
            }

            if (e is BefriendedHumanEvent fe)
            {
                CurrentState.Description += $"Loves to eat {fe.FavoriteFood} at a {fe.FavoriteColor} table";

                CurrentState.IsFriend = true;
                
            }

            if (e is GotIlleHumanEvent ile)
            {
                CurrentState.MedicalHistory.Add(ile.Sickness);

                CurrentState.IsHealthy = false;
            }
            
            if(e is GotHealedHumanEvent he)
            {
                CurrentState.IsHealthy = true;
            }

            CurrentState.LastNewsUpdateAbout = e.Timestamp;
        }

        [NoStreamIdApi]
        public void Register(string name, string surname, string job)
        {
            if (!this.IsPristine())
            {
                throw new HumanException("You can not Re-Create a human that already has been created!");
            }

            var sid = Guid.NewGuid();

            this.Initialize(sid);
            
            ActionTaken(new IntroducedHumanEvent
            {
                Job = job,
                Name = name,
                Surname = surname,
                Timestamp = DateTime.Now,
                Id = sid
            });
        }

        public void KnowCapabilities(string weakness, string power)
        {
            ActionTaken(new TrustedHumanEvent
            {
                Weakness = weakness,
                Timestamp = DateTime.Now,
                SuperPower = power
            });
        }

        public void KnowTaste(string favoriteFood, string favoriteColor)
        {
            ActionTaken(new BefriendedHumanEvent
            {
                Timestamp = DateTime.Now,
                FavoriteColor = favoriteColor,
                FavoriteFood = favoriteFood
            });
        }

        public void SetSickness(string issue)
        {

            var isSick = string.IsNullOrWhiteSpace(issue); 
            
            if (isSick)
            {
                ActionTaken(new GotHealedHumanEvent
                {
                    Timestamp = DateTime.Now
                });
            }
            else
            {
                if (CurrentState.IsHealthy)
                {
                    throw new HumanException(CurrentState, "Cant get better than fine!");
                }
                ActionTaken(new GotIlleHumanEvent
                {
                    Timestamp = DateTime.Now,
                    Sickness = issue
                });
            }
        }
    }
}