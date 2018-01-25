# FABRIC

Build Status:  [![CircleCI](https://circleci.com/gh/lexwebb/Fabric/tree/dev.svg?style=svg)](https://circleci.com/gh/lexwebb/Fabric/tree/dev)

## Huh?
Fabric will be a set of librarys and client/server tools for centrally storing/modifying and version controlling settings and configuration for applications within an organisation.

The aim is to provide a data storage mechanism that is human readable, performant and easily consumable.

## How?
### Database
A custom data storage platform will be created that stores all data in JSON format in raw flat files.
This, combined with GIT for versioning control and long term storage willl provide a versatile and human readable database format.

### REST based API interface
Rather then rely on custom librarys or drivers, fabric will be communicable through a REST based API,
allowing for platform and software agnostic access.