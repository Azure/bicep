class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  url "https://github.com/Azure/bicep/releases/download/v0.1.37-alpha/bicep-osx-x64"
  sha256 "9502397b2680db391135d9558efefb1e14c3b1c4cd6385d8b53e50bf49b2dcbf"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
