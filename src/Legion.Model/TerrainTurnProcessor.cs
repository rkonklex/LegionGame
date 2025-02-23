using AwaitableCoroutine;
using Legion.Model.Types;
using Legion.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Legion.Model
{
    public class TerrainTurnProcessor : ITerrainTurnProcessor
    {
        private readonly TerrainActionContext _context;

        public TerrainTurnProcessor(TerrainActionContext context)
        {
            _context = context;
        }

        public async Coroutine ProcessTurn()
        {
            var userCharacters = _context.UserArmy.Characters;
            var enemyCharacters = _context.EnemyArmy.Characters;

            var numCharacters = Math.Max(userCharacters.Count, enemyCharacters.Count);
            for (int i = 0; i < numCharacters; i++)
            {
                if (i < userCharacters.Count)
                {
                    ProcessUserTurn(userCharacters[i]);
                }
                if (i < enemyCharacters.Count)
                {
                    ProcessEnemyTurn(enemyCharacters[i]);
                }
            }

            await Coroutine.Yield();
        }

        private void ProcessUserTurn(Character character)
        {
            switch (character.CurrentAction)
            {
                case CharacterActionType.None:
                    character.Bob = 0;
                    break;

                case CharacterActionType.Move:
                    ProcessMove(character);
                    break;

                case CharacterActionType.Attack:
                    ProcessAttack(character);
                    break;

                default:
                    throw new NotImplementedException("Unsupported action type");
            }
        }

        private void ProcessEnemyTurn(Character character)
        {
            switch (character.CurrentAction)
            {
                case CharacterActionType.None:
                    GiveTheOrder(character);
                    break;

                case CharacterActionType.Move:
                    if (!ProcessMove(character))
                    {
                        RedirectStuckCharacter(character);
                    }
                    if (GlobalUtils.Rand(20) == 1)
                    {
                        GiveTheOrder(character);
                    }
                    break;

                case CharacterActionType.Attack:
                    if (!ProcessAttack(character))
                    {
                        RedirectStuckCharacter(character);
                    }
                    if (GlobalUtils.Rand(10) == 1)
                    {
                        GiveTheOrder(character);
                    }
                    break;

                default:
                    throw new NotImplementedException("Unsupported action type");
            }
        }

        private static readonly int[] AnimFrameSequence = { 0, 1, 0, 2 };

        private bool ProcessMove(Character character)
        {
            var x1 = character.X;
            var y1 = character.Y;
            var dx = character.Target.X - x1;
            var dy = character.Target.Y - y1;
            var speed = Math.Clamp(character.Speed / 10, 1, 7);
            var hasMoved = false;

            var animSpeed = Math.Clamp(3 - character.Speed / 10, 1, 3);
            var nextAnimFrame = (character.CurrentAnimFrame + 1) % (4 * animSpeed);
            var animFrame = AnimFrameSequence[nextAnimFrame / animSpeed];
            var bob = 6;

            if (Math.Abs(dx) > 4)
            {
                var tx = dx < 0 ? -17 : 17;
                if (!_context.HitTest(x1 + tx, y1, out _))
                {
                    x1 += Math.Sign(dx) * speed;
                    bob = (dx < 0 ? 3 : 9) + animFrame;
                    hasMoved = true;
                }
            }

            if (Math.Abs(dy) > 4)
            {
                var ty = dy < 0 ? -21 : 2;
                if (!_context.HitTest(x1, y1 + ty, out _))
                {
                    y1 += Math.Sign(dy) * speed;
                    bob = (dy < 0 ? 0 : 6) + animFrame;
                    hasMoved = true;
                }
            }

            if (Math.Abs(dx) <= 4 && Math.Abs(dy) <= 4)
            {
                character.OrderIdle();
                hasMoved = true;
            }

            character.X = x1;
            character.Y = y1;
            character.CurrentAnimFrame = nextAnimFrame;
            character.Bob = bob;
            return hasMoved;
        }

        private bool ProcessAttack(Character character)
        {
            throw new NotImplementedException();
        }

        private void RedirectStuckCharacter(Character character)
        {
            var x2 = character.Target.X + GlobalUtils.Rand(120) - 60;
            var y2 = character.Target.Y + GlobalUtils.Rand(100) - 50;
            character.OrderMoveTo(Math.Clamp(x2, 20, 620), Math.Clamp(y2, 20, 510));
        }

        private void GiveTheOrder(Character character)
        {
            var nearestTarget = _context.UserArmy.FindNearestCharacter(character.X, character.Y, out var distanceToTarget);

            switch (character.Aggression)
            {
                case < 50:
                    GiveRandomMoveOrder(character);
                    break;

                case <= 100:
                    if (distanceToTarget < 50)
                    {
                        character.OrderAttack(nearestTarget);
                    }
                    else
                    {
                        GiveRandomMoveOrder(character);
                    }
                    break;

                case <= 150:
                    if (distanceToTarget < 50)
                    {
                        character.OrderAttack(nearestTarget);
                    }
                    else
                    {
                        if (GlobalUtils.Rand(1) == 0)
                        {
                            character.Aggression = 90;
                        }
                        else
                        {
                            character.Aggression = 155;
                        }
                    }
                    break;

                default:
                    character.OrderAttack(nearestTarget);
                    break;
            }
        }

        private void GiveRandomMoveOrder(Character character)
        {
            var x2 = GlobalUtils.Rand(600) + 20;
            var y2 = GlobalUtils.Rand(450) + 50;
            if (!_context.HitTest(x2, y2, out _))
            {
                character.OrderMoveTo(x2, y2);
            }
        }
    }
}