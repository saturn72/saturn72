# saturn72
Saturn Technologies main repository for building microservices

##What are microservices?
Microservices are small independent web services with simple and well defined functinoality. 

##Why microservices?
By striping your web service offer you simplify the development process, reduce the dev time and maintainance time, gain better control on code flow and in general acheive more with less resources as your development efforts are more focused on specific goal and not supporting generic functionality.

##How Saturn72 supports microservices?
Saturn72 code base supports modules which are the extension point for the framework. Creating `Saturn72AppBase` derivative and launching it (using the `Saturn72AppBase` base type `Start` method), loads all modules and starts them.
The `Saturn72AppBase` direative you create is your microservices container.

To create your own functionality you need to create a module performing the required buisiness logic.
See the examples of this repository to get more details on how to create and consume microservice developed using Saturn72.

##What Saturn72 is?
Framework to develop medium to large applications.using .Net framework (C#) using best practices to define and develop both frontend, backend and app services.

##What Saturn72 is not?
Saturn72 is not boilerplate framework but framework and collection of standarts to develop multiple applications and services sharing the same development practice and code base.
Another thing Saturn72 is not is framework to develop POC's or Ad-Hoc functionality, this is better managed and developed using simple C# projects and solution as you may find in multiple tutorials on-line.

##Who should use Saturn72 framework?
Since Saturn72 uses advanced coding practices, tools, patterns and terms we recomend intermidiate or above development knowledge.
Either way we recomend you refer to our wiki knowledge base [developer-must-knows](https://github.com/saturn72/saturn72/wiki/Developer-Must-Knows) before getting started.

#What will you gain using Saturn72?
- Advanced developemnt practives
- S.O.L.I.D based framework
- Advanced unit testing framework 
- Base line for any medium to large application development
- Well defined solution structure
- Modules and components developed by Saturn72 team
- Reducing the development time 
- Focusing on your business logic and not on infrastructure creation and adjustments
- You are not really dependant on Saturn72 - your modules and applications has specific integrationpoints with Saturn72 codebase that can easily replaced.
