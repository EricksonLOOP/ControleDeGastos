using CGD.APP.DTOs.Auth;
using CGD.APP.DTOs.User;
using CGD.APP.Mappers;
using CGD.CrossCutting.Exceptions;
using CGD.CrossCutting.Security;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;

namespace CGD.APP.Services.Auth;

public class AuthServices(IUserRepository userRepository, PasswordHash passwordHash) : IAuthServices
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly PasswordHash _passwordHash = passwordHash;

    public async Task SignupAsync(AuthSignupDto authSignupDto)
    {
        // Regra de negocio: bloqueia email duplicado para preservar unicidade de identidade.
        var existingUser = await _userRepository.GetByEmailAsync(authSignupDto.Email);
        if (existingUser is not null)
            throw new ArgumentException("Email já cadastrado");

        var user = UserMapper.AuthToEntity(authSignupDto, _passwordHash.HashPassword(authSignupDto.Password));
        await _userRepository.AddAsync(user);
    }

    public async Task<UserDto> LoginAsync(AuthLoginDto authLoginDto)
    {
        // Fluxo de autenticacao: busca usuario, valida hash e retorna Unauthorized em credencial invalida.
        var user = await _userRepository.GetByEmailAsync(authLoginDto.email) ?? throw new UserNotFoundException();
        if (!_passwordHash.Verify(authLoginDto.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Credenciais inválidas");

        return UserMapper.ToDto(user);
    }
}