using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.BLL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardDeck.Queuetrigger
{
    public class CardDeckQueueTrigger
    {
        private readonly ICardService _cardService;
        private readonly IDeckService _deckService;
        public CardDeckQueueTrigger(ICardService cardService, IDeckService deckService)
        {
            _cardService = cardService;
            _deckService = deckService;
        }

        [FunctionName("CardDeckQueueTrigger")]
        public async Task RunAsync([QueueTrigger("carddeckqueue", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            KeyValuePair<string, string> myItem = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(myQueueItem);
            try
            {
                //this switch uses the key of the keyvalue pair to determine what DTO or class to use to deserialize the json object
                switch (myItem.Key)
                {
                    case "CreateCard":
                        await _cardService.CreateCardInDeckAsync(JsonConvert.DeserializeObject<CardBaseWithCardIDAndDeckIDDTO>(myItem.Value));
                        break;
                    case "UpdateCard":
                        await _cardService.UpdateCardInDeckAsync(JsonConvert.DeserializeObject<CardBaseWithCardIDDTO>(myItem.Value));
                        break;
                    case "DeleteCard":
                        await _cardService.DeleteCardInDeckAsync(JsonConvert.DeserializeObject<CardWithCardIDAndDeckIDDTO>(myItem.Value));
                        break;
                    case "CreateDeck":
                        await _deckService.CreateDeckAsync(JsonConvert.DeserializeObject<DeckBaseWithIDDTO>(myItem.Value));
                        break;
                    case "UpdateDeck":
                        await _deckService.UpdateDeckAsync(JsonConvert.DeserializeObject<DeckBaseWithIDDTO>(myItem.Value));
                        break;
                    case "DeleteDeck":
                        await _deckService.DeleteDeckFromTeamAsync(JsonConvert.DeserializeObject<DeckWithDeckIDAndTeamIDDTO>(myItem.Value));
                        break;
                }
            }
            catch (EntityNotFoundException ex)
            {
                log.LogInformation(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                log.LogInformation(ex.Message);
            }
        }
    }
}
