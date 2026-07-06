# Agent Instructions

This is a small, simple game. Keep the implementation just as simple.
Do not write code to introduce heavy enterprise abstractions or patterns. 
IF a pattern is missing that would beenfit the project, inform the user and link to a resource to learn about it.

## Game Design Pillars
1. The game is a physics combat engine; as such, the physics engine is the most important part of the game, and kept simple yet satisfying
2. We should lean on existing systems within the Unity game engine, do not feel the need to over-engineer our own systems, especially physics
3. You are allowed to use serialized fields through the inspector, but immediately inform the user what was added, and how it functions
4. We are working first and foremost to build a simple and fun MVP of the game, and then we can iterate on it to add more features and polish

## General rule

Prefer the boring, direct solution. This project does not need enterprise
patterns. If you're about to add an abstraction layer, ask first.

## Agent Guidelines
- When asked to update default values, ensure that you update the prefab and the backing C# default values
- When asked to change values, default to changing the prefab values, NOT the values in a particular scene