using AutoMapper;
using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zxcvbn;
using Repository;
using Utils.Exceptions;
using BCrypt.Net;

namespace Service

{
   public class UserService:IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Register(NewUserDto newUserDto)
        {
            // 1. בדיקת חוזק סיסמה
            var result = Zxcvbn.Core.EvaluatePassword(newUserDto.Password);
            if (result.Score <= 2)
                throw new ValidationException("Password is too weak. Please use a stronger password.");

            // 2. בדיקה אם המייל כבר קיים
            if (await _repository.IsEmailExists(newUserDto.Email))
                throw new ValidationException("This email address is already in use.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password);
            newUserDto.Password = hashedPassword;
            // 3. מיפוי ושמירה
            var customer = _mapper.Map<Customer>(newUserDto);
            return await _repository.AddUser(customer);
        }

        public async Task<CustomerDto> Login(LoginDto loginDto)
        {
            var user = await _repository.GetUserByEmail(loginDto.Email);

            if (user != null && user.IsBlocked)
            {
                throw new ForbiddenException("החשבון חסום לצמיתות עקב ניסיון לשנות מחירים בהזמנה.");
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                  
                throw new ValidationException("Invalid email or password.");
            

            // 3. מיפוי ל-CustomerDto (כדי לא להחזיר את הסיסמה המוצפנת ללקוח)
            return _mapper.Map<CustomerDto>(user);
        }

        public async Task Update(int id, UpdateUserDto updateUserDto)
        {
            // 1. שליפת המשתמש הקיים
            var existingUser = await _repository.GetUserById(id);
            if (existingUser == null)
                throw new NotFoundException($"User with ID {id} not found.");

            // 2. בדיקה אם הוא מנסה לשנות למייל שכבר תפוס ע"י מישהו אחר
            var userWithSameEmail = await _repository.GetUserByEmail(updateUserDto.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != id)
                throw new ValidationException("This email is already taken by another user.");

            // 3. עדכון השדות (אפשר ידנית או בעזרת AutoMapper)
            _mapper.Map(updateUserDto, existingUser);

            // 4. שמירה
            await _repository.UpdateUser(existingUser);
        }

    }
}
