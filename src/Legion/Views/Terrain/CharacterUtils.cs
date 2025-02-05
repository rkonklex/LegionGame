using System;
using Legion.Model.Types;
using Legion.Utils;
using Microsoft.Xna.Framework;

namespace Legion.Views.Terrain
{
    public class CharactersUtils
    {
        public static Character FindCharacterAtPosition(Army army, Point p)
        {
            foreach (var character in army.Characters)
            {
                var bounds = GetCharacterBounds(character);
                if (bounds.Contains(p))
                {
                    return character;
                }
            }
            return null;
        }

        public static Rectangle GetCharacterBounds(Character character)
        {
            // TODO: provide correct bounds
            //var textures = CharactersImagesLoader.Get(character.Type);
            //var frame = textures[character.CurrentAnimFrame];
            //var charBounds = new Rectangle(character.X, character.Y, frame.Width, frame.Height);
            //32x45
            var charBounds = new Rectangle(character.X, character.Y, 32, 45);

            return charBounds;
        }

        //TODO: need to be refactored. It must probably also check collision with buildings and other terrain things
        public static bool CheckCollision(Character character, Point newPosition, Army userArmy, Army enemyArmy)
        {
            var b = GetCharacterBounds(character);
            var pos = new Point(newPosition.X, newPosition.Y + b.Height);

            var isCollision = new Func<Character, bool>(c =>
                {
                    if (c.Id != character.Id)
                    {
                        var bounds = GetCharacterBounds(c);
                        if (bounds.Contains(pos))
                        {
                            return true;
                        }
                    }
                    return false;
                });

            foreach (var c in userArmy.Characters)
            {
                if (isCollision(c))
                {
                    return true;
                }
            }
            foreach (var c in enemyArmy.Characters)
            {
                if (isCollision(c))
                {
                    return true;
                }
            }

            return false;
        }
    }

}