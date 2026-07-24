namespace SmartCollegeComplaintPortal.Domain.Constants
{
    public class ClaimConstants
    {
        /// <summary>
        /// JWT "sub" claim — contains the user's integer ID.
        /// Matches JwtRegisteredClaimNames.Sub in the token generation.
        /// </summary>
        public const string UserId = "sub";

        /// <summary>
        /// JWT "email" claim.
        /// </summary>
        public const string Email = "email";

        public const string Purpose = "purpose";
    }
}
