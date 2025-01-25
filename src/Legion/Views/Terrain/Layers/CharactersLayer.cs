using System.ComponentModel;
using System.Linq;
using Gui.Elements;
using Gui.Input;
using Gui.Services;
using Legion.Controllers.Terrain;
using Legion.Model;
using Legion.Model.Types;
using Legion.Utils;
using Microsoft.Xna.Framework;

namespace Legion.Views.Terrain.Layers
{
    public class CharactersLayer : Layer
    {
        private readonly ITerrainController _terrainController;
        private readonly ILegionConfig _legionConfig;
        private readonly CharactersActions _actions;
        private bool _wasMouseDown;

        public CharactersLayer(IGuiServices guiServices,
            ITerrainController terrainController,
            ILegionConfig legionConfig) : base(guiServices)
        {
            _legionConfig = legionConfig;
            _terrainController = terrainController;
            _actions = new CharactersActions();
        }

        public Army EnemyArmy { get; set; }

        public Army UserArmy { get; set; }

        public Character SelectedCharacter { get; set; }

        public bool IsAlive => UserArmy.Characters.Count > 0;

        public bool IsPaused { get; set; }// = true;

        public CharacterActionType? CurrentMode { get; set; }

        public override void OnShow()
        {
            UserArmy = _actions.UserArmy = _terrainController.UserArmy = ((TerrainActionContext) Parent.Context).UserArmy;
            EnemyArmy = _actions.EnemyArmy = _terrainController.EnemyArmy = ((TerrainActionContext) Parent.Context).EnemyArmy;

            CharactersUtils.InitArmyPostion(UserArmy, 1, 1, 0);
            CharactersUtils.InitArmyPostion(EnemyArmy, 1, 1, 1);

            foreach (var enemyChar in EnemyArmy.Characters)
            {
                enemyChar.TargetX = GlobalUtils.Rand(_legionConfig.WorldWidth);
                enemyChar.TargetY = GlobalUtils.Rand(_legionConfig.WorldHeight);
                enemyChar.CurrentAction = CharacterActionType.Move;
            }
        }

        public override void OnHide()
        {
            foreach (var character in UserArmy.Characters)
            {
                character.CurrentAction = CharacterActionType.None;
            }
            foreach (var character in EnemyArmy.Characters)
            {
                character.CurrentAction = CharacterActionType.None;
            }

            SelectedCharacter = null;
            UserArmy = _actions.UserArmy = null;
            EnemyArmy = _actions.EnemyArmy = null;
        }

        protected override void OnUpdate()//GameTime gameTime)
        {
            base.OnUpdate();

            var gameTime = 20;

            if (IsPaused)
            {
                if (SelectedCharacter == null)
                {
                    SelectedCharacter = UserArmy.Characters.FirstOrDefault();
                }
            }
            else
            {
                foreach (var userChar in UserArmy.Characters)
                {
                    switch (userChar.CurrentAction)
                    {
                        case CharacterActionType.Move:
                            _actions.Move(userChar, gameTime);
                            break;
                        case CharacterActionType.Attack:
                        case CharacterActionType.Speak:
                            _actions.Attack(userChar, gameTime);
                            break;
                    }
                }

                foreach (var enemyChar in EnemyArmy.Characters)
                {
                    switch (enemyChar.CurrentAction)
                    {
                        case CharacterActionType.None:
                            _actions.GiveTheOrder(enemyChar);
                            break;
                        case CharacterActionType.Move:
                            _actions.Move(enemyChar, gameTime);
                            if (GlobalUtils.Rand(21) == 1)
                            {
                                _actions.GiveTheOrder(enemyChar);
                            }
                            break;
                        case CharacterActionType.Attack:
                        case CharacterActionType.Speak:
                            _actions.Attack(enemyChar, gameTime);
                            if (GlobalUtils.Rand(11) == 1)
                            {
                                _actions.GiveTheOrder(enemyChar);
                            }
                            break;
                    }
                }
            }
        }

        protected override void OnClick(HandledEventArgs args)
        {
            base.OnClick(args);

            if (HandleClick(InputManager.GetMousePostion()))
            {
                args.Handled = true;
            }
        }

        public bool HandleClick(Point mousePosition)
        {
            var handled = HandleTerrainClicked(mousePosition);
            if (handled) return true;

            var enemyChar = CharactersUtils.FindCharacterAtPosition(EnemyArmy, mousePosition);
            if (enemyChar != null)
            {
                HandleCharacterClicked(enemyChar);
                return true;
            }

            var userChar = CharactersUtils.FindCharacterAtPosition(UserArmy, mousePosition);
            if (userChar != null)
            {
                HandleCharacterClicked(userChar);
                return true;
            }
            return false;
        }

        private void HandleCharacterClicked(Character character)
        {
            if (CurrentMode.HasValue)
            {
                switch (CurrentMode.Value)
                {
                    case CharacterActionType.Attack:
                    case CharacterActionType.Speak:
                        SelectedCharacter.CurrentAction = CurrentMode.Value;
                        SelectedCharacter.TargetType = CharacterTargetType.Character;
                        SelectedCharacter.TargetId = character.Id;
                        break;
                }

                CurrentMode = null;
            }
            else
            {
                if (UserArmy.Characters.Contains(character))
                {
                    SelectedCharacter = character;
                }
            }
        }

        private bool HandleTerrainClicked(Point position)
        {
            var handled = false;

            if (CurrentMode.HasValue)
            {
                switch (CurrentMode.Value)
                {
                    case CharacterActionType.Move:
                        SelectedCharacter.CurrentAction = CharacterActionType.Move;
                        SelectedCharacter.TargetType = CharacterTargetType.Position;
                        SelectedCharacter.TargetX = position.X;
                        SelectedCharacter.TargetY = position.Y;
                        handled = true;
                        break;
                    case CharacterActionType.Shoot:
                        SelectedCharacter.CurrentAction = CharacterActionType.Shoot;
                        SelectedCharacter.TargetType = CharacterTargetType.Position;
                        SelectedCharacter.TargetX = position.X;
                        SelectedCharacter.TargetY = position.Y;
                        handled = true;
                        break;
                }

                if (handled)
                {
                    CurrentMode = null;
                }
            }

            return handled;
        }

        protected override void OnDraw()
        {
            DrawCharacters();

            if (_terrainController.IsPaused)
            {
                DrawMarkers();
                DrawSelectors();
            }
        }

        private void DrawCharacters()
        {
            //TODO from where UserArmy/EnemyArmy should be readed? from controller or Parent.Context?
            //terrainController.UserArmy.Characters)
            var context = (TerrainActionContext) Parent.Context;
            foreach (var userChar in context.UserArmy.Characters)
            {
                DrawCharacter(userChar);
            }

            foreach (var enemyChar in context.EnemyArmy.Characters)
            {
                DrawCharacter(enemyChar);
            }
        }

        private void DrawCharacter(Character character)
        {
            var imgName = GuiServices.ImagesStore.GetNames().FirstOrDefault(n =>
                n.EndsWith("." + character.Type.Name)
            );
            var images = GuiServices.ImagesStore.GetImages(imgName);
            var frame = images[character.CurrentAnimFrame];
            GuiServices.BasicDrawer.DrawImage(frame, character.X, character.Y);
        }

        private void DrawMarkers()
        {
            // foreach (var userChar in terrainController.UserArmy.Characters)
            // {
            //     var textures = CharactersImagesLoader.Get(userChar.Type);
            //     var frame = textures[userChar.CurrentAnimFrame];
            //     var x = userChar.X + (frame.Width / 2) - (marker.Width / 2);
            //     var y = userChar.Y - 20;
            //     spriteBatch.Draw(marker, new Vector2(x, y), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0);
            // }
        }

        private void DrawSelectors()
        {
            // spriteBatch.Draw(selectorGreen, new Vector2(SelectedCharacter.X, SelectedCharacter.Y + 20), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0);

            // if (SelectedCharacter.CurrentAction != CharacterActionType.None)
            // {
            //     var x = 0;
            //     var y = 0;
            //     switch (SelectedCharacter.CurrentAction)
            //     {
            //         case CharacterActionType.Move:
            //         case CharacterActionType.Shoot:
            //             x = SelectedCharacter.TargetX;
            //             y = SelectedCharacter.TargetY;
            //             break;
            //         case CharacterActionType.Attack:
            //         case CharacterActionType.Speak:
            //             var targetChar = EnemyArmy.Characters.Find(c => c.Id == SelectedCharacter.TargetId);
            //             if (targetChar == null)
            //             {
            //                 targetChar = UserArmy.Characters.Find(c => c.Id == SelectedCharacter.TargetId);
            //             }
            //             x = targetChar.X;
            //             y = targetChar.Y + 20;
            //             break;
            //     }
            //     spriteBatch.Draw(selectorOrange, new Vector2(x, y), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0);
            // }
        }

    }

}