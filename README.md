# Web Application API Documentation

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
