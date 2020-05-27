using System;
using GlmNet;

namespace editor
{
    class Camera
    {
        // view port - rendering plane
        vec4 mViewport;
        // camera position
        private vec3 mPosition;
        // eye target location
        private vec3 mTarget;
        // up vector of the camera, for rolling orientation purposes
        private vec3 mUp;

        private mat4 mLookAt;
        private mat4 mOrtho;

        private bool mNeedToRecalculate = false;

        public Camera()
        {
            mPosition = new vec3(0, 0, 10);
            mTarget = new vec3(0, 0, 0);
            mUp = new vec3(0, 1, 0);
            mLookAt = mat4.identity() ;
            mViewport = new vec4(0,0,1,1);
            mOrtho = mat4.identity();
            mNeedToRecalculate = true;
        }

        public void MoveCameraOnX(float distance)
        {
            mPosition.x += distance;
            mNeedToRecalculate = true;
        }

        public void MoveCameraOnY(float distance)
        {
            mPosition.y += distance;
            mNeedToRecalculate = true;
        }

        public void MoveCameraOnZ(float distance)
        {
            mPosition.z += distance;
            mNeedToRecalculate = true;
        }

        internal void SetViewport(int width, int height)
        {
            mViewport = new vec4(0, 0, width, height);
            mNeedToRecalculate = true;
        }

        public void StrifeCameraOnX(float distance)
        {
            mPosition.x += distance;
            mTarget.x += distance;
            mNeedToRecalculate = true;
        }

        public void StrifeCameraOnY(float distance)
        {
            mPosition.y += distance;
            mTarget.y += distance;
            mNeedToRecalculate = true;
        }

        public void StrifeCameraOnZ(float distance)
        {
            mPosition.z += distance;
            mTarget.z += distance;
            mNeedToRecalculate = true;
        }

        public void StrifeCamera(vec3 distance)
        {
            mPosition += distance;
            mTarget += distance;
            mNeedToRecalculate = true;
        }

        public vec3 GetCameraPosition()
        {
            return mPosition;
        }

        public mat4 GetLookAt()
        {
            recalculate();

            return mLookAt;
        }

        public mat4 GetOrtho()
        {
            recalculate();

            return mOrtho;
        }

        public mat4 GetProjectionView()
        {
            recalculate();

            return mOrtho * mLookAt;
        }

        public vec4 GetViewport()
        {
            return mViewport;
        }

        public void recalculate()
        {
            if (mNeedToRecalculate)
            {
                int d = (int)mPosition.z;
                int k = d / 2;
                int width = (int)mViewport.z;
                int height = (int)mViewport.w;

                mOrtho = GlmNet.glm.ortho(
                    -width / 2 + k,
                     width / 2 - k,
                    -height / 2 + k,
                     height / 2 - k,
                     1,
                     100);
                mLookAt = glm.lookAt(mPosition, mTarget, mUp);
                mNeedToRecalculate = false;
            }
        }
    }
}
