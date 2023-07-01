# BinCollection Function App

Azure Function that automatically sends you a text when it's your bin collection the next day. Also has the capability to ingest a text message and responds with when the next bin collection is. This uses Twilio as the text service.

This calls off to the council API with a UPRN for the property - this could be expanded to support multiple council APIs as a lot of them support lookup by UPRN see [UKBinCollectionData](https://github.com/robbrad/UKBinCollectionData)
