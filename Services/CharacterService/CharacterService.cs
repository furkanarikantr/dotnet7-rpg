using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;


namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        //Automapper enjekte ediyoruz.
        private readonly IMapper _mapper;
        //Db'yi enjekte ediyoruz.
        private readonly DataContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        //Tüm verileri liste şeklinde getirme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.Where(c => c.User!.Id == GetUserId()).ToListAsync();
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
            //var dbCharacter = await _context.Characters.Where(c => c.User!.Id == GetUserId()).FirstOrDefaultAsync(c => c.Id == id);
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        //Veri ekleme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();  //Veri tabanına yazmamızı sağlar.

            serviceResponse.Data = await _context.Characters.Where(u => u.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        //Veri Güncelleme
        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try //yöntem 1
            {
                var dbCharacter = await _context.Characters
                .Include(c => c.User)   //İlgili nesnelere erişmek için Include kullanılır.
                .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id /*&& c.User!.Id == GetUserId() //Yöntem 1*/);
                //var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);

                if(dbCharacter is null || dbCharacter.User!.Id != GetUserId())
                {
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");   //yöntem 2
                }
                //  else if(dbCharacter.User!.Id != GetUserId())
                //  {
                //      throw new Exception($"Kullanıcı ID'si ile '{dbCharacter.User!.Id}' Karakter UserId'si '{GetUserId()}' aynı değil.");   //yöntem 2
                //  }
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

        //Veri silme.
        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                //var character = characters.FirstOrDefault(c => c.Id == id);
                var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
                if(dbCharacter is null)
                {
                    throw new Exception($"Character with Id '{id}' not found.");   //yöntem 2
                }
                //characters.Remove(character);
                _context.Characters.Remove(dbCharacter);
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters.Where(u => u.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
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