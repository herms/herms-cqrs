# Introduction
Herms CQRS was created by Herms (Herman Crawfurd Svensen) in an attempt to learn and gain experience with CQRS and Event Sourcing (ES). In addition, I wanted to get more experience with the Azure product line. 

> Herms CQRS is by no means finished, and I will at this point introduce breaking changes with almost every build. Not in an attempt to be mean, of course...

Herms CQRS consists of the core `herms.cqrs` package, and some optional packages that provide implementations of some concepts defined in the core.

* `herms.cqrs.ninject`
* `herms.cqrs.azure`
* `herms.cqrs.file` (for testing purposes, but still - be my guest!)

# Features

* CQRS/ES building blocks
* Assembly scanning
* Auto-registration
* Azure provider (Table Storage, DocumentDB)
* Ninject provider

# Getting started

The core package `herms.cqrs` provides the building blocks for creating a CQRS and ES enabled app, the most basic ones being: `Aggregate`, `Command` and `ICommandHandler`, and `IEvent` and `IEventHandler` types. It also contains the classes and interfaces needed to create a registry of command handlers, event handlers, and an assembly scanner that can be used to ease the registration process. 

In short, `herms.cqrs` is designed like this:

1. Accept a command
2. Find a command handler in the registry
3. Handle the command, update the aggregate, and publish events
4. Find relevant event handlers in the registry
5. Handle the event(s) in the handler(s)

## Brief intro to core types

### Command
The `Command` type has a few `Guid` properties, the most important being the Id of the `Aggregate` that the command is targeting.

### ICommandHandlerRegistry
Some place in your code, you will accept a `Command` of some type, and you must find the correct handler for it. `herms.cqrs` is designed so that there can only exist one handler per command type. The `ICommandHandlerRegistry` is responsible for finding the correct handler based on the `Command` type with the method `ResolveHandler`.

### ICommandHandler
The `ICommandHandler` interface defines one method: `Handle`. In the handler you will get the aggregate, and change the aggregate according to the command. The `ICommandHandler` will need a `IEventDispatcher`.

### IEventDispatcher
The `IEventDispatcher` defines the method `Publish`. There is no implementation of `IEventDispatcher` included in any of the packages yet, but this could be something that sends a message on a queue, writes a file, or whatever.

### IEventHandlerRegistry
In the receiving end, you will need something that picks up published events. This component will need a `IEventHandlerRegistry`. The `IEventHandlerRegistry` defines a method called `ResolveHandlers` (notice the plural), and will return a collection of `IEventHandler`s wrapped inside a `EventHandlerCollection`. You could call `Handle` on each of the `IEventHandler` objects returned, or...

### EventHandlerCollection
The `EventHandlerCollection` has a method `Handle` which will call `Handle` on every `IEventHandler` in the collection (regardless of the result of each one). The collective results will be presented in a `EventHandlerResults` object.

## Registration
The core package contains a namespace that 
The `herms.cqrs.ninject` package contains code that will with _one_ method call scan your assemblies for events, commands and handlers. 