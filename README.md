#DHS.CQRSlite
## The framework
DHS.CQRSlite is a small CQRS and Eventsourcing Framework forked from CQRSlite. It is written in C# and targets .NET 4.5 CQRSlite originated as a CQRS sample project Greg Young.
This code is located at http://github.com/gregoryyoung/m-r

CQRSlite has been made with pluggability in mind. So every standard implementation should be interchangeable with a custom one if needed.

##Getting started
A sample project is located with the code, this shows a common usage scenario of the framework. There are some features of CQRSlite, such as snapshotting that the sample does not show. These features are only documented through the tests.

The project should compile without any setup in .NET 4.5 or Mono 3.10.0. 

##Features
* Command sending and event publishing
* Unit of work through session with aggregate tracking
* Repository for getting and saving aggregates
* Optimistic concurrency checking
* In process bus with autoregistration of handlers
* Snapshotting
* Caching with concurrency checks and updating to latest version

## Features added in DHS.CQRSlite
* Event versioning
* downgraded to .Net 4.5
* compatible with Unity

##Installation
To install DHS.CqrsLite, it you could either download the files and copy whats needed into your project, you can clone this project and reference it, or you can download releases from nuget. You will have to implement your own eventstore, an adapter to use an existing eventstore should be trivial.

##License 
Copyright 2014 Gaute Magnussen

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
