using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gui.Services;
using Legion.Localization;

namespace Legion.Views.Map.Controls
{
    public class ArmyOrdersWindow : ButtonsListWindow
    {
        public ArmyOrdersWindow(IGuiServices guiServices,
            ITexts texts,
            bool isTerrainActionButtonVisible,
            bool isRecruitButtonVisible) : base(guiServices)
        {
            var dict = new Dictionary<string, Action<HandledEventArgs>>
            {
                {
                    texts.Get("armyMenu.move"), args =>
                    {
                        MoveClicked?.Invoke(args);
                        Close();
                    }
                },
                {
                    texts.Get("armyMenu.fastMove"), args =>
                    {
                        FastMoveClicked?.Invoke(args);
                        Close();
                    }
                },
                {
                    texts.Get("armyMenu.attack"), args =>
                    {
                        AttackClicked?.Invoke(args);
                        Close();
                    }
                }
            };

            if (isRecruitButtonVisible)
            {
                dict.Add(texts.Get("armyMenu.recruit"), args =>
                {
                    RecruitClicked?.Invoke(args);
                    Close();
                });
            }
            else
            {
                dict.Add(texts.Get("armyMenu.hunt"), args =>
                {
                    HuntClicked?.Invoke(args);
                    Close();
                });
            }

            dict.Add(texts.Get("armyMenu.camp"), args =>
            {
                CampClicked?.Invoke(args);
                Close();
            });

            dict.Add(texts.Get("armyMenu.equipment"), args =>
            {
                EquipmentClicked?.Invoke(args);
                Close();
            });

            if (isTerrainActionButtonVisible)
            {
                dict.Add(texts.Get("armyMenu.action"), args =>
                {
                    ActionClicked?.Invoke(args);
                    Close();
                });
            }

            dict.Add(texts.Get("armyMenu.exit"), args =>
            {
                ExitClicked?.Invoke(args);
                Close();
            });

            ButtonNames = dict;
        }

        public event Action<HandledEventArgs> MoveClicked;

        public event Action<HandledEventArgs> FastMoveClicked;

        public event Action<HandledEventArgs> AttackClicked;

        public event Action<HandledEventArgs> RecruitClicked;

        public event Action<HandledEventArgs> HuntClicked;

        public event Action<HandledEventArgs> CampClicked;

        public event Action<HandledEventArgs> EquipmentClicked;

        public event Action<HandledEventArgs> ActionClicked;

        public event Action<HandledEventArgs> ExitClicked;

        public override void Draw()
        {
            base.Draw();

            //TODO: selected action marker 
            /*
               If TRYB>0
                  Text OKX+65,OKY-4+18*TRYB,"@"
               End If 
               If TRYB=0
                  Text OKX+65,OKY-4+18*5,"@"
               End If 
            */
        }
    }
}