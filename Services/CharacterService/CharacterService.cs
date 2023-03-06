using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>
        {
            new Character(),
            new Character{
                Id = 1,
                Name = "Sam"
            }
        };

        //Automapper injecte ediyoruz.
        private readonly IMapper _mapper;

        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        //Tüm verileri liste şeklinde getirme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }
        
        //Id'ye göre verileri getirme.
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            // var selectedById = characters.SingleOrDefault(c => c.Id == id);
            // return Ok(selectedById);

            //null dönebilir uyarısı ile karşılaşmamak için bu kısmı ekliyoruz. Farklı çözümler var, ileride konuşacağız.
            //var character = characters.FirstOrDefault(c => c.Id == id);
            // if(serviceResponse is not null)
            // {
            //     return serviceResponse;
            // }
            // throw new Exception("Character Not Found!");

            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var character = characters.FirstOrDefault(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            return serviceResponse;
        }

        //Veri ekleme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(c => c.Id) + 1;
            characters.Add(character);
            serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        //Veri Güncelleme
        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try //yöntem 1
            {
                
                var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);

                if(character is null)
                {
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");   //yöntem 2
                }

                _mapper.Map(updatedCharacter,character);

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                var character = characters.FirstOrDefault(c => c.Id == id);
                if(character is null)
                {
                    throw new Exception($"Character with Id '{id}' not found.");   //yöntem 2
                }
                characters.Remove(character);
                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        
    }
}