"""PixAI: starter utilities for Persian language-model experiments."""

from __future__ import annotations


def project_info() -> dict[str, str]:
    """Return basic project metadata."""
    return {"name": "PixAI", "language": "fa", "stage": "starter"}


if __name__ == "__main__":
    for key, value in project_info().items():
        print(f"{key}: {value}")
