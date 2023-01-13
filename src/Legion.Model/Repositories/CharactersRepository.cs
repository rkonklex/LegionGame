using Legion.Model.Types;
using Legion.Model.Types.Definitions;
using Legion.Utils;

namespace Legion.Model.Repositories
{
    public class CharactersRepository : ICharactersRepository
    {
        private readonly IDefinitionsRepository _definitionsRepository;

        public CharactersRepository(IDefinitionsRepository definitionsRepository)
        {
            _definitionsRepository = definitionsRepository;
        }

        public Character CreateWarrior(RaceDefinition type)
        {
            var character = CreateBaseCharacter(type);
            character.Magic = character.MagicMax = GlobalUtils.Rand(5) + type.Magic;
            return character;
        }

        public Character CreateNpc(CharacterDefinition type, int power)
        {
            return type switch
            {
                RaceDefinition warriorType => CreateExperiencedWarrior(warriorType, power),
                CreatureDefinition creatureType => CreateCreature(creatureType),
                _ => CreateBaseCharacter(type),
            };
        }

        private Character CreateBaseCharacter(CharacterDefinition type)
        {
            var speed = GlobalUtils.Rand(10) + type.Speed;
            var energy = (GlobalUtils.Rand(20) + type.Energy) * 3;
            return new Character
            {
                Type = type,
                Name = NamesGenerator.Generate(),
                Strength = GlobalUtils.Rand(10) + (type.Strength / 2),
                Speed = speed,
                SpeedMax = speed,
                Energy = energy,
                EnergyMax = energy,
            };
        }

        private Character CreateExperiencedWarrior(RaceDefinition type, int power)
        {
            var character = CreateWarrior(type);
            character.Strength += GlobalUtils.Rand(power);
            character.Resistance = GlobalUtils.Rand(power / 2) + power / 2;
            character.Experience = GlobalUtils.Rand(power / 2) + power / 2;
            return character;
        }

        private Character CreateCreature(CreatureDefinition type)
        {
            var character = CreateBaseCharacter(type);
            character.Aggression = 150 + GlobalUtils.Rand(60);
            if (type.Spell != 0)
            {
                var spell = _definitionsRepository.GetItemByOldIndex(type.Spell);
                character.Magic = character.MagicMax = spell.Magic * 5;
                character.Equipment.Backpack[0] = new Item { Type = spell };
            }
            character.Resistance = type.Resistance / 2 + GlobalUtils.Rand(type.Resistance / 2);
            character.Experience = GlobalUtils.Rand(30);
            return character;
        }
    }
}