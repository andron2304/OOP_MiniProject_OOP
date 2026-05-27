# Iteration 3 Summary

## Code Coverage Goals
- Achieve at least 85% coverage on domain and service logic.
- Cover validation branches for track creation and playlist modification.
- Cover exception handling paths for invalid input and repository failures.

## Eliminated Code Smells
1. Replaced duplicated exception handling logic with centralized custom domain exceptions.
2. Reduced tight coupling between service and repository by relying on `IRepository<T>` abstractions.
3. Simplified playlist mutation logic to avoid nested conditionals and make behavior easier to read and test.

## Remaining Risks for Lab 37
- Risk of incomplete repository integration when moving from in-memory storage to a persistent backend.
- Risk of missing validation edge cases for track metadata and playlist state transitions.
- Risk of insufficient coverage of failure paths if repository exceptions are not exercised in tests.
