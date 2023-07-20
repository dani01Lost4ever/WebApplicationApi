# Web Application API Documentation

This project is part of a bigger project including 2 other teams the Front-end and Mobile App teams.

## Technology Stack

The project was built:

- C# for the API
- Azure Cosmos DB as the Database.

Our objective in creating this API was to provide access to the data to other groups in the projects.

<div style="display: flex; gap: 10px;">
<img src="./icons/Microsoft-Azure.svg" alt="Microsoft Azure" style="width: 35px">

<img src="./icons/Azure-Cosmos-DB.svg" alt="Azure CosmosDB icon" style="width: 35px">

<img src="./icons/Csharp.svg" alt="Csharp icon" style="width: 35px">
</div>

## Base URL

`https://questionuser.azurewebsites.net/api/`

## API Endpoints

### 1. Get list of questions

- URL: /People/LastQuestions
- Method: POST
- Parameters:
  - quantity (integer, required)
- Description: Retrieve a list of the last <b>n</b> questions.

- Example: `POST https://questionuser.azurewebsites.net/api/People/LastQuestions?quantity=2`

### 2. Get list of questions between 2 dates

- URL: /People/LastQuestions
- Method: POST
- Parameters:
  - <b>quantity</b> (integer, required)
  - <b>startDate</b> (date, required)
  - <b>endDate</b> (date, required)
- Description: Retrieve a list of questions asked between two specific dates.

- Example: `POST https://questionuser.azurewebsites.net/api/People/GetQuestionsTimeOffset?startDate=2000-01-01&endDate=2030-01-01&quantity=3`

### 3. Get list of questions between 2 dates and state of question

- URL: /People/LastQuestions
- Method: POST
- Parameters:
  - <b>quantity</b> (integer, required)
  - <b>startDate</b> (date, required)
  - <b>endDate</b> (date, required)
  - <b>statoDellaDomanda</b> (integer, required)
- Description: Retrieve a list of questions asked between two specific dates, filtered by their state.

- Example: `POST https://questionuser.azurewebsites.net/api/People/GetQuestionsTimeOffsetAndState?startDate=2020-01-01&endDate=2025-01-01&quantity=1&statoDellaDomanda=1`

### 4. Get list of questions in a province

- URL: /People/LastQuestions
- Method: POST
- Parameters:
  - <b>quantity</b> (integer, required)
  - <b>province</b> (string, required)
- Description: Retrieve a list of questions asked in a particular province.

- Example: `POST https://questionuser.azurewebsites.net/api/People/ByProvince?quantity=4&province=Vicenza`

### 5. Get number of questions per province

- URL: /People/LastQuestions
- Method: GET
- Description: Retrieve the number of questions asked per province.

- Example: `GET https://questionuser.azurewebsites.net/api/People/userprovince`

### Team

This project was made by:

- [dani01Lost4ever](https://github.com/dani01Lost4ever)

- [CavaliereDavid](https://github.com/CavaliereDavid)

- [catgrasso](https://github.com/catgrasso)

- [Rikymeggio1234](https://github.com/Rikymeggio1234)

- [godwin-17](https://github.com/godwin-17)
