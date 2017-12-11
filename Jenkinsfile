pipeline {
    agent none

    stages {
        stage('Build') {
			agent any
            steps {
                dir('./src/') {
                    sh 'sudo docker-compose -f docker-compose.ci.build.yml up'
                    sh 'sudo docker-compose -f docker-compose.ci.build.yml down --remove-orphans'
                }
            }
        }
		// stage('BuildWebApp') {
		// 	agent { dockerfile true }
        //     steps {
        //         echo 'Building..'
        //     }
        // }		
        stage('Test') {
            steps {
                echo 'Testing..'
            }
        }
        stage('Deploy') {
            when {
              expression {
                currentBuild.result == null || currentBuild.result == 'SUCCESS' 
              }
            }
            steps {
                echo "publish ${env.BUILD_ID} on ${env.JENKINS_URL}"
            }
        }
    }
}