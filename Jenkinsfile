pipeline {
  agent any
  tools { nodejs 'Node24' }
  stages {
    stage('Checkout') {
      steps {
        git branch: 'main',
            url: 'https://github.com/Nawinvignes/WeatherApplication.git'
      }
    }
    stage('Build .NET Backend') {
      steps {
        dir('backend-final') {
          bat 'dotnet restore'
          bat 'dotnet build --configuration Release'
        }
      }
    }
    stage('Build Angular Frontend') {
      steps {
        dir('frontend-final') {
          bat 'npm ci'
          bat 'npm run build -- --configuration production'
        }
      }
    }
    stage('SonarQube Analysis') {
      steps {
        withSonarQubeEnv('SonarQube-Local') {
          dir('backend-final') {
            bat 'dotnet sonarscanner begin /k:"weather-api-backend" /d:sonar.host.url="http://localhost:9000"'
            bat 'dotnet build'
            bat 'dotnet sonarscanner end'
          }
        }
      }
    }
  }
}
