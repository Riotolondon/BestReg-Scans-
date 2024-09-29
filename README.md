Setting Up Firebase Credentials
Obtain Firebase Credentials JSON File:
Access the Firebase Console.
Select your project and navigate to Settings > Service Accounts.
Click Generate New Private Key to download the JSON file. Store it securely.
Set Environment Variable on Windows:
Open PowerShell and execute:
[System.Environment]::SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\path\to\your\firebase-credentials.json", [System.EnvironmentVariableTarget]::User)
Replace the path with your actual JSON file location.
Verify Environment Variable:
Run echo $env:GOOGLE_APPLICATION_CREDENTIALS in PowerShell to confirm the setup.
Pushing Changes to GitHub
Exclude Sensitive Files:
Add to .gitignore:
BestReg/Secrets/*.json
Remove Secrets from History:
If committed, remove with:
git rm --cached BestReg/Secrets/newchilddb-firebase-adminsdk-3trwt-cd44449665.json
git commit --amend --no-edit
Force Push (if necessary):
Clean history with git filter-branch or BFG Repo-Cleaner, then:
git push origin master --force
Use GitHub Secrets for CI/CD:
Store sensitive data in GitHub Secrets for secure access in workflows.
Moving to Production
Secure Storage:
Store the JSON file securely on the production server with restricted access.
Set Environment Variable:
Use server environment configuration tools to set GOOGLE_APPLICATION_CREDENTIALS.
Documentation:
Document the production setup process to ensure consistency and security.
By following these guidelines, you can maintain a secure and efficient workflow for both development and production environments.

Video for user roles : https://youtu.be/Y6DCP-yH-9Q
