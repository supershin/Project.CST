using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;
using Project.ConstructionTracking.Web.Repositories;
using System.Transactions;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;

namespace Project.ConstructionTracking.Web.Services
{
    public class ChatInBoxService : IChatInBoxService
    {
        private readonly IChatInBoxRepo _IChatInBoxRepo;

        public ChatInBoxService(IChatInBoxRepo IChatInBoxRepo)
        {
            _IChatInBoxRepo = IChatInBoxRepo;
        }

        public List<ChatInBoxModel.GetlistChatInBox> GetListChatInBox(ChatInBoxModel.GetlistChatInBox filterData)
        {
            var ListChatInBox = _IChatInBoxRepo.GetListChatInBox(filterData);
            return ListChatInBox;
        }

        public insertInBox InsertUnitFormInbox(insertInBox En)
        {
            // Define transaction options with a timeout of 5 Minutes
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted, // Set the isolation level to ReadCommitted
                Timeout = TimeSpan.FromMinutes(5) // Set the timeout to 5 Minutes
            };

            // Create a new transaction scope with the defined options
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    // Insert data into the UnitFormInbox
                    _IChatInBoxRepo.InsertUnitFormInbox(En);

                    // Create a response object after the insertion
                    insertInBox createdInbox = new insertInBox
                    {
                        UserID = En.UserID,
                        UnitFormID = En.UnitFormID,
                        RoleID = En.RoleID,
                        FormID = En.FormID,
                        TextInbox = En.TextInbox
                    };

                    scope.Complete(); // Mark the transaction as complete
                    return createdInbox;
                }
                catch (Exception ex)
                {
                    // Handle the exception and log it if necessary
                    throw new Exception("An error occurred while inserting data into the UnitFormInbox.", ex);
                }
            } // The TransactionScope is automatically disposed here
        }

    }
}
