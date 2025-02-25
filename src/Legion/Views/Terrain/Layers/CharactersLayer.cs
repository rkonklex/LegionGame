using System.ComponentModel;
using System.Linq;
using AwaitableCoroutine;
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
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ILegionConfig _legionConfig;

        public CharactersLayer(IGuiServices guiServices,
            ITerrainController terrainController,
            ICoroutineRunner coroutineRunner,
            ILegionConfig legionConfig) : base(guiServices)
        {
            _legionConfig = legionConfig;
            _terrainController = terrainController;
            _coroutineRunner = coroutineRunner;
        }

        public Army EnemyArmy => _terrainController.EnemyArmy;

        public Army UserArmy => _terrainController.UserArmy;

        public Character SelectedCharacter { get; set; }

        public CharacterActionType? CurrentMode { get; set; }

        private Coroutine _turnTask;

        public override void OnShow()
        {
            _terrainController.SetupTerrainAction((TerrainActionContext)Parent.Context);
            _turnTask = _coroutineRunner.Create(_terrainController.StartTerrainAction);
        }

        public override void OnHide()
        {
            SelectedCharacter = null;

            _terrainController.EndTerrainAction();
            _turnTask = null;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_terrainController.IsPaused)
            {
                if (SelectedCharacter == null)
                {
                    SelectedCharacter = UserArmy.Characters.FirstOrDefault();
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

            if (EnemyArmy.HitTest(mousePosition.X, mousePosition.Y, out var enemyChar))
            {
                HandleCharacterClicked(enemyChar);
                return true;
            }

            if (UserArmy.HitTest(mousePosition.X, mousePosition.Y, out var userChar))
            {
                HandleCharacterClicked(enemyChar);
                return true;
            }

            return false;
        }

        private void HandleCharacterClicked(Character character)
        {
            var isUserCharacter = UserArmy.Characters.Contains(character);
            if (CurrentMode.HasValue && !isUserCharacter)
            {
                switch (CurrentMode.Value)
                {
                    case CharacterActionType.Attack:
                        SelectedCharacter.OrderAttack(character);
                        break;

                    case CharacterActionType.Speak:
                        // TODO
                        break;
                }

                CurrentMode = null;
            }
            else if (!CurrentMode.HasValue && isUserCharacter)
            {
                SelectedCharacter = character;
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
                        SelectedCharacter.OrderMoveTo(position.X, position.Y);
                        handled = true;
                        break;

                    case CharacterActionType.Shoot:
                        // TODO
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
            foreach (var body in _terrainController.Scenery.Bodies)
            {
                DrawCharacter(body);
            }

            foreach (var userChar in _terrainController.UserArmy.Characters)
            {
                DrawCharacter(userChar);
            }

            foreach (var enemyChar in _terrainController.EnemyArmy.Characters)
            {
                DrawCharacter(enemyChar);
            }
        }

        private void DrawCharacter(Character character)
        {
            var images = GuiServices.ImagesStore.GetImages(character.Type.Img);
            var frame = images[character.Bob];
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