# AI in Games Course

Welcome to the **AI in Games Course**! This university course focuses on applying reinforcement learning and pathfinding algorithms within the Unity game engine. The course is designed to give students hands-on experience with essential AI techniques and their applications in game environments. 

## Project Overview

This repository contains the project components for the course. The material is organized into three main sections:

1. **Rolling Ball Environment**  
   Students will implement a custom Deep Q-learning (DQN) algorithm from scratch using Python, PyTorch, and OpenAI Gym. The objective is to train a rolling ball to reach a specific target on a square ground.
<p align="center">
  <img src="https://github.com/user-attachments/assets/a371a067-a85a-4257-b0f1-305e63eb8525" alt="roller_ball_DQN" width="500"/>
</p>

2. **Autonomous Car Environment**  
   This section involves implementing and training a car agent using Unity's `ml-agents` package. Students will apply the Proximal Policy Optimization (PPO) and Advantage Actor-Critic (A2C) algorithms to enable the car to drive autonomously around various tracks.
<p align="center">
  <img src="https://github.com/user-attachments/assets/f29e1d96-6bf6-4cf3-ac18-d3d774d28580" alt="Autonomous car track" width="500"/>
</p>

3. **Navigation System Creator**  
   Building on the autonomous car project, students will implement a navigation system for the car agent within Unity. The task includes developing and experimenting with search algorithms such as Breadth-First Search (BFS), DFS, A*, and Dijkstra to navigate the car around obstacles and reach a destination.
<p align="center">
  <img src="https://github.com/user-attachments/assets/45323313-e22d-4cd8-ac6f-d80293959e44" alt="Navigation map" width="500"/>
</p>

## Repository Structure

- `AI_in_games_unity/`: Contains Unity scripts and scenes for both the rolling ball and autonomous car projects.
- `Rolling_ball_Python/`: Includes Python scripts required for the custom Deep Q-learning implementation using PyTorch and Gym.
- `results/`: Contains configuration files and performance graphs of the car agent.
<p align="center">
<img width="500" alt="track4_result" src="https://github.com/user-attachments/assets/627ac504-5713-4db8-9c9f-6a037c24deca">
</p>

## Installation and Setup

To get started with the repository:

1. **Unity Setup**  
   - Install [Unity](https://unity.com/) and the [ml-agents](https://github.com/Unity-Technologies/ml-agents) package for reinforcement learning integration.
   - Open the `AI_in_games_unity/` folder in Unity to access the scripts and scenes for the rolling ball and autonomous car environments.

2. **Python Environment**  
   - Make sure you have Python installed, along with the necessary packages:
     ```bash
     pip install torch gym matplotlib
     ```
   - Navigate to the `Rolling_ball_Python/` folder to run the Python code for the Deep Q-learning algorithm.

## Usage

1. **Rolling Ball**  
   - Run the scripts in `Rolling_ball_Python/` to train the DQN model on the rolling ball environment.

2. **Autonomous Car**  
   - Launch the Unity scene in `AI_in_games_unity/` and use the `ml-agents` package to train the car using PPO or A2C.

3. **Navigation System**  
   - Implement and test different search algorithms within Unity to guide the car's navigation.

## Features

- Custom Deep Q-learning implementation from scratch
- Autonomous driving using PPO and A2C algorithms
- Search algorithms for navigation, including BFS, DFS, A*, and Dijkstra

## Contributing

We welcome contributions! Please fork the repository and submit a pull request for any bug fixes or improvements.

