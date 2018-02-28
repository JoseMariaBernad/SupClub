using Firebase.Auth;
using SupClubLib.Model;
using SupClubLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLib.Services
{
    public class AuthService : IAuthService
    {
        private FirebaseAuthProvider _authProvider;
        private FirebaseAuthLink _auth;

        public IUserRepository UserRepository { get; set; }

        public ClubUser CurrentUser { get; private set; }

        public AuthService(string firebaseApiKey)
        {
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
        }

        public async Task<AuthResult> CreateUserAsync(ClubUser user, string password, bool sendVerificationEmail)
        {
            string displayName = user.Name + " " + user.Surname;
            try
            {
                _auth = await _authProvider.CreateUserWithEmailAndPasswordAsync(
                                user.Email, password, displayName, sendVerificationEmail);
                user.UserId = _auth.User.LocalId;
                user.IsEmailVerified = _auth.User.IsEmailVerified;
                // TODO: Add user to database if boolean optional parameter 
                //       saveUser is true (pending to add parameter)
                return new AuthResult { User = user, Result = "Success" };
            }
            catch (Exception ex)
            {
                string reason = "Unknown reason";
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    reason = "Mail Exists";
                return new AuthResult { User = user, Result = "Error: " + reason};
            }
        }

        // TODO: refactor this method and its overload to eliminate duplicate code.
        public async Task<AuthResult> LoginWithEmailAndPasswordAsync(string email, 
                                                                        string password, 
                                                                        bool enforceValidation = true,
                                                                        bool saveUser = true)
        {
            ClubUser user = null;
            try
            {
                _auth = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
                await _auth.RefreshUserDetails();
                user = await UserRepository.FindUserByIdAsync(_auth.User.LocalId);
                if (user == null)
                {
                    user = new ClubUser { Email = email };
                }
                user.UserId = _auth.User.LocalId;
                user.IsEmailVerified = _auth.User.IsEmailVerified;
                if (!user.IsEmailVerified && enforceValidation)
                {

                    return new AuthResult { User = user, Result = "Error: Please, validate your email." };
                }
                else
                {
                    if (saveUser)
                    {
                        await UserRepository.SaveUserAsync(user);
                    }
                    CurrentUser = user;
                    return new AuthResult { User = user, Result = "Success" };
                }
            }
            catch (Exception)
            {
                return new AuthResult { User = user, Result = "Error: Invalid credentials" };
            }
        }

        public async Task<AuthResult> LoginWithEmailAndPasswordAsync(ClubUser user, 
                                                                        string password,
                                                                        bool enforceValidation = true,
                                                                        bool saveUser = true)
        {
            try
            {
                _auth = await _authProvider.SignInWithEmailAndPasswordAsync(user.Email, password);
                await _auth.RefreshUserDetails();
                user.UserId = _auth.User.LocalId;
                user.IsEmailVerified = _auth.User.IsEmailVerified;
                if (!user.IsEmailVerified && enforceValidation)
                {

                    return new AuthResult { User = user, Result = "Error: Please, validate your email." };
                }
                else
                {
                    if (saveUser)
                    {
                        await UserRepository.SaveUserAsync(user);
                    }
                    CurrentUser = user;
                    return new AuthResult { User = user, Result = "Success" };
                }
            }
            catch (Exception)
            {
                return new AuthResult { User = user, Result = "Error: Invalid credentials" };
            }
        }

        public async Task ResetPassword(string email)
        {
            await _authProvider.SendPasswordResetEmailAsync(email);
        }

        public async Task<string> GetFirebaseTokenAsync()
        {
            if (_auth != null)
                return _auth.FirebaseToken;
            else
                throw new NotImplementedException(); //TODO: Authenticate anonymously if token does not exist.
        }
    }

    public class AuthResult
    {
        public ClubUser User { get; set; }
        public string Result { get; set; }
    }
}
