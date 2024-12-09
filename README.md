# Dynamic Ragdoll

A project focused on training a dynamic ragdoll to perform various tasks using reinforcement learning. Starting with standing, the project will progress to tasks like walking, running, and climbing stairs.

## Motivation

This project is heavily inspired by the Unity ML-Agents walker. The Unity **character model** was used as the foundation for this project, with custom reinforcement learning logic designed to train the ragdoll from scratch. The ultimate goal is to explore the dynamics of simulated movements and test the adaptability of these learned behaviors on a human-like model.

## Python Environment

- **Python Version**: `3.10.11`
- Ensure you have this specific Python version installed to avoid compatibility issues.

## Goals

1. Train a ragdoll to stand stably using reinforcement learning.
2. Progressively introduce more complex tasks:
   - Walking
   - Running
   - Climbing stairs
3. Adapt the trained logic to a human-like model to test the transferability of learned behaviors.

## Dependencies

This project uses the following libraries:
- **PyTorch**: For implementing reinforcement learning algorithms.
- **Unity ML-Agents**: For simulation and reinforcement learning integration.

## Installation

Set up the project environment with the required dependencies:

### 1. **Create and activate a virtual environment**:
```bash
python -m venv venv
source venv/bin/activate        # On macOS/Linux
venv\Scripts\activate           # On Windows
```

### 2. **Install Dependencies**

#### For CUDA-Enabled Devices:
Run the following command to install PyTorch with CUDA 12.4 support along with other dependencies:

```bash
pip install torch --index-url https://download.pytorch.org/whl/cu124
pip install -r requirements-cuda.txt
```

#### For CPU-Only Devices:
Run the following command to install PyTorch for CPU-only execution along with other dependencies:

```bash
pip install -r requirements-cpu.txt
```

---

## How It Works

1. **Setup**: A ragdoll is modeled within a physics-based environment using Unity ML-Agents' character model.
2. **Training**: Reinforcement learning algorithms are applied to teach the ragdoll:
   - To balance and stand stably.
   - To optimize movement for stability and efficiency.

## Progress

- **Standing**: Currently in progress.
- **Walking**: Planned.
- **Running**: Planned.
- **Climbing Stairs**: Planned.

## Inspiration

This project draws inspiration from Unity ML-Agents' walker, using their **character model** while developing a custom logic pipeline to train and adapt dynamic movements.

## Contributing

Feel free to fork the repository and submit pull requests if you'd like to contribute to this project!

## License

None.

---
