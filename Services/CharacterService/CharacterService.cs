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

        //Automapper enjekte ediyoruz.
        private readonly IMapper _mapper;
        //Db'yi enjekte ediyoruz.
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        //Tüm verileri liste şeklinde getirme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
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
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            //var character = characters.FirstOrDefault(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        //Veri ekleme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();  //Veri tabanına yazmamızı sağlar.
            serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        //Veri Güncelleme
        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try //yöntem 1
            {
                var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                //var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);

                if(dbCharacter is null)
                {
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");   //yöntem 2
                }

                _mapper.Map(updatedCharacter,dbCharacter);

                dbCharacter.Name = updatedCharacter.Name;
                dbCharacter.HitPoints = updatedCharacter.HitPoints;
                dbCharacter.Strength = updatedCharacter.Strength;
                dbCharacter.Defense = updatedCharacter.Defense;
                dbCharacter.Intelligence = updatedCharacter.Intelligence;
                dbCharacter.Class = updatedCharacter.Class;

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
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
                //var character = characters.FirstOrDefault(c => c.Id == id);
                var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if(dbCharacter is null)
                {
                    throw new Exception($"Character with Id '{id}' not found.");   //yöntem 2
                }
                //characters.Remove(character);
                _context.Characters.Remove(dbCharacter);
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
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