# Project Setup Guide

## Setting Up Firebase Credentials

- **Obtain Firebase Credentials JSON File**:
  1. Access the [Firebase Console](https://console.firebase.google.com/).
  2. Select your project and navigate to **Settings** > **Service Accounts**.
  3. Click **Generate New Private Key** to download the JSON file. Store it securely.

- **Set Environment Variable on Windows**:
  1. Open PowerShell and execute:
   powershell as Admin
 [System.Environment]::SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\path\to\your\firebase-credentials.json", [System.EnvironmentVariableTarget]::User)


  2. Replace the path with your actual JSON file location.

- **Verify Environment Variable**:
  - Run the following in PowerShell to confirm the setup:
powershell
echo $env:GOOGLE_APPLICATION_CREDENTIALS



## Pushing Changes to GitHub

- **Exclude Sensitive Files**:
  - Add the following to your `.gitignore` file:
bash 
git rm --cached BestReg/Secrets/newchilddb-firebase-adminsdk-3trwt-cd44449665.json git commit --amend --no-edit


- **Use GitHub Secrets for CI/CD**:
  - Store sensitive data in GitHub Secrets for secure access in workflows.

## Moving to Production

- **Secure Storage**:
  - Store the JSON file securely on the production server with restricted access.

- **Set Environment Variable**:
  - Use server environment configuration tools to set `GOOGLE_APPLICATION_CREDENTIALS`.

- **Documentation**:
  - Document the production setup process to ensure consistency and security.

By following these guidelines, you can maintain a secure and efficient workflow for both development and production environments.


Video for user roles : https://youtu.be/Y6DCP-yH-9Q
