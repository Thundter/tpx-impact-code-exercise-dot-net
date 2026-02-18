# URL Shortener Coding Exercise

## Getting Started Locally

1. run `Python create_db.py` to create database for projectdotnet add package Swashbuckle.AspNetCore


## notes

__todo__ - currently untested 

### .Net 10 & NSwag incompatability

- installing the latest on the both of these causes many packages to auto downgrade, which in turn causes the update to fail
- documentation for using both together is currently incomplete as .Net 10 is less that 2 months old
- using both required and downgrade of .Net. Decided to use version 8.0

### Swashbuckle.AspNetCore.Annotations 

- this call was made to install `dotnet add package Swashbuckle.AspNetCore.Annotations --version 10.1.3` 
- do not upgrade to version 10.1.4 as it auto downgrades to version 10.1.3, which in turn causes the update to fail

## Task

Build a simple **URL shortener** in .Net and React

It should:

- Accept a full URL and return a shortened URL.
- A shortened URL should have a randomly generated alias.
- Allow a user to **customise the shortened URL** if they want to (e.g. user provides `my-custom-alias` instead of a random string).
- Persist the shortened URLs across restarts.
- Expose a **decoupled web frontend built using React** that demonstrates interaction with the API. The frontend does not need to be visually complex, but should clearly show your approach to **frontend structure, state handling, and user interaction**. You may use supporting frameworks or libraries (e.g. Next.js, Material-UI, Tailwind CSS, GOV.UK Design System, etc.) to speed up development if you wish.
- Expose a **RESTful API** to perform create/read/delete operations on URLs.  
  → Refer to the provided [`openapi.yaml`](./openapi.yaml) for API structure and expected behaviour.
- Include the ability to **delete a shortened URL** via the API.
- **Have tests**.
- Be containerised (e.g. Docker).
- Include instructions for running locally.

## Rules

- Fork the repository and work in your fork. Do not push directly to the main repository.
- There is no time limit, we want to see something you are proud of. We would like to understand roughly how long you spent on it though.
- **Commit often with meaningful messages.**
- Write tests.
- The API should validate inputs and handle errors gracefully.
- The Frontend should show errors from the API appropriately.
- Use the provided [`openapi.yaml`](./openapi.yaml) as the API contract.
- Focus on clean, maintainable code.
- AI tools (e.g., GitHub Copilot, ChatGPT) are allowed, but please **do not** copy-paste large chunks of code. Use them as assistants, not as a replacement for your own work. We will be asking.

## Deliverables

- Working software.
- Decoupled web frontend using React.
- RESTful API matching the OpenAPI spec.
- Tests.
- A git commit history that shows your thought process.
- Dockerfile.
- README with:
  - How to build and run locally.
  - Example usage (frontend and API).
  - Any notes or assumptions.
